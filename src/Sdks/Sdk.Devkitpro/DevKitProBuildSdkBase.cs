using System.Xml.Linq;
using Brewery.Sdk.DevKitPro.BuildTasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.Sdk.DevKitPro;

public abstract class DevKitProBuildSdkBase : IBuildSdk
{
    public abstract string Name { get; }

    public abstract string CompilerDirectory { get; }
    public abstract string CompilerPrefix { get; }

    public DirectoryInfo? DevKitProPath { get; private set; }

    private ILogger<DevKitProBuildSdkBase> m_logger = null!;
    private IServiceProvider m_services = null!;

    public virtual void Initialize(IServiceProvider services)
    {
        m_logger = services.GetRequiredService<ILogger<DevKitProBuildSdkBase>>();
        m_services = services;

        var envPath = Environment.GetEnvironmentVariable("DEVKITPRO");
        if (envPath is not null && Path.IsPathRooted(envPath))
        {
            var validateDir = Path.Combine(envPath, CompilerDirectory);
            if (!Directory.Exists(validateDir))
            {
                m_logger.Error("Location specified does not seem to be a valid Devkitpro install");
            }

            DevKitProPath = new DirectoryInfo(envPath);
            m_logger.Debug($"Devkitpro install: {DevKitProPath.FullName}");
        }
        else
        {
            m_logger.Error("No Devkitpro install found. Is the DEVKITPRO environment variable set correctly?");
        }
    }

    public IBuildSdkProjectSettings? ReadSdkSettings(XElement rootElement)
    {
        var settings = InternalReadSdkSettings(rootElement);

        rootElement.ReadProperty<string>("SystemLib", value => settings.SystemLib = value)
            .ReadListProperty<string>("AdditionalLibs", "Lib", value => settings.AdditionalLibs.AddRange(value));

        return settings;
    }

    protected virtual DevKitProBuildSdkProjectSettings InternalReadSdkSettings(XElement rootElement) => new();

    public BuildResult PerformBuild(GameProject project)
    {
        var dispatcher = new BuildCommandDispatcher(m_services);

        dispatcher.RunParallel(project.GenerateSourceBuildTasks(out var sourceBuildArtifacts));
        dispatcher.RunTask(LinkTask.Generate(project, sourceBuildArtifacts, out var elfFile));
        var postBuildTask = GetPostBuildBinaryTask(project, elfFile);
        if (postBuildTask is not null)
            dispatcher.RunTask(postBuildTask);

        return dispatcher.ExecuteTasks();
    }

    public abstract CompileInfo GetCompileCommand(GameProject project, SourceBuildRule rule, string sourceFile);
    public abstract CompileInfo GetLinkCommand(GameProject project, IEnumerable<FileInfo> objectFiles);

    protected virtual IBuildTask? GetPostBuildBinaryTask(GameProject project, FileInfo elfFile)
    {
        return null;
    }
}