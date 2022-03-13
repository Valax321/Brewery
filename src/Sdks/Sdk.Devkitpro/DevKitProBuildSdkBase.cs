using System.Xml.Linq;
using Brewery.Sdk.DevKitPro.BuildTasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Utility;
using Brewery.ToolSdk.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// Base class for all the DevKitPro SDKs.
/// </summary>
public abstract class DevKitProBuildSdkBase : IBuildSdk
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <summary>
    /// The directory the SDK's compiler is located in.
    /// </summary>
    public abstract string CompilerDirectory { get; }

    /// <summary>
    /// The GCC prefix for compiler executables.
    /// </summary>
    public abstract string CompilerPrefix { get; }

    /// <summary>
    /// The path DevKitPro is installed at.
    /// </summary>
    public DirectoryInfo DevKitProPath { get; private set; } = default!;

    private ILogger<DevKitProBuildSdkBase> m_logger = null!;
    private IServiceProvider m_services = null!;

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IBuildSdkProjectSettings? CreateSdkSettings()
    {
        return new DevKitProBuildSdkProjectSettings
        {
            LibrarySearchPaths = new List<string> { DevKitProPath.FullName }
        };
    }

    /// <inheritdoc />
    public void ReadSdkSettings(XElement rootElement, IBuildSdkProjectSettings? settings, bool isConfiguration)
    {
        if (settings is not DevKitProBuildSdkProjectSettings dkpSettings)
            throw new InvalidOperationException();

        InternalReadSdkSettings(rootElement, dkpSettings);

        rootElement.ReadProperty<string>("SystemLib", value => dkpSettings.SystemLib = value)
            .ReadListProperty<string>("AdditionalLibs", "Lib", value => dkpSettings.AdditionalLibs.AddRange(value))
            .ReadListProperty<string>("LibrarySearchPaths", "SearchPath", value => dkpSettings.LibrarySearchPaths.AddRange(value));

        rootElement.ReadProperty<string>("OptimizationLevel", value =>
        {
            if (Enum.TryParse<GCCOptimizationLevel>(value, out var level))
            {
                dkpSettings.OptimizationLevel = level;
            }
            else
            {
                m_logger.Error($"Unknown GCC optimization level {value}.\nSee https://gcc.gnu.org/onlinedocs/gcc/Optimize-Options.html for a list of valid optimization levels.");
            }
        });
    }

    /// <summary>
    /// Read dkp-specific project settings.
    /// </summary>
    /// <param name="rootElement">The <see cref="XElement"/> at the root of the document.</param>
    /// <param name="settings">The settings object to read into.</param>
    /// <returns></returns>
    protected virtual void InternalReadSdkSettings(XElement rootElement, DevKitProBuildSdkProjectSettings settings) { }

    /// <inheritdoc />
    public BuildResult PerformBuild(GameProject project)
    {
        var dispatcher = new BuildCommandDispatcher(m_services);

        dispatcher.RunParallel(project.GenerateAssetBuildTasks(out var assetBuildArtifacts));
        project.AssetBuildArtifacts = assetBuildArtifacts;

        // Here we need to actually the asset build before source compilation occurs
        // Otherwise the build artifacts don't actually exist and will be missed by the file matching
        // in the source build rules.
        var result = dispatcher.ExecuteTasks();
        if (result == BuildResult.Failed)
            return result;

        // Cursed concatenation of normal source files and generated .c and .s compiled asset files.
        dispatcher.RunParallel(project.GenerateSourceBuildTasks(out var sourceBuildArtifacts)
            .Concat(project.GenerateSourceBuildTasks(out var compiledAssetsBuildArtifacts, 
                project.IntermediateDirectory.GetSubDirectory("assets"))));
        project.SourceBuildArtifacts = sourceBuildArtifacts.Concat(compiledAssetsBuildArtifacts);

        dispatcher.RunTask(LinkTask.Generate(project, out var elfFile));
        var postBuildTask = GetPostBuildBinaryTask(project, elfFile);
        if (postBuildTask is not null)
            dispatcher.RunTask(postBuildTask);

        return dispatcher.ExecuteTasks();
    }

    /// <summary>
    /// Get commandline arguments for compiling source code files using this SDK.
    /// </summary>
    /// <param name="project">The project being compiled.</param>
    /// <param name="rule">Source rule being generated.</param>
    /// <param name="sourceFile">Path to the file being compiled.</param>
    /// <returns><see cref="CompileInfo"/> describing the compilation command.</returns>
    public abstract CompileInfo GetCompileCommand(GameProject project, SourceBuildRule rule, string sourceFile);

    /// <summary>
    /// Get commandline arguments for linking object files using this SDK.
    /// </summary>
    /// <param name="project">The project being compiled.</param>
    /// <param name="objectFiles">Object files that are being linked.</param>
    /// <returns><see cref="CompileInfo"/> describing the linker command.</returns>
    public abstract CompileInfo GetLinkCommand(GameProject project, IEnumerable<FileInfo> objectFiles);

    /// <summary>
    /// Gets a <see cref="IBuildSdk"/> task that should be executed after linking/binary generation is complete.
    /// </summary>
    /// <param name="project">The project being compiled.</param>
    /// <param name="elfFile"><see cref="FileInfo"/> describing the ELF file generated by the linker.</param>
    /// <returns></returns>
    protected virtual IBuildTask? GetPostBuildBinaryTask(GameProject project, FileInfo elfFile)
    {
        return null;
    }
}