using System.Runtime.InteropServices;
using System.Xml.Linq;
using Brewery.Sdk.NativeToolchain.MSVC;
using Brewery.Sdk.NativeToolchain.Tasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Utility;
using Brewery.ToolSdk.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.Sdk.NativeToolchain;

/// <summary>
/// Toolchain for building c/c++ code for the host CPU.
/// </summary>
public class NativeToolchainBuildSdk : IBuildSdk
{
    internal const string SdkName = "NativeToolchain";

    /// <inheritdoc />
    public string Name => "NativeToolchain";

    /// <summary>
    /// Path to where the compiler is.
    /// </summary>
    public string CompilerPath { get; private set; } = string.Empty;

    /// <summary>
    /// Path to where the linker is.
    /// </summary>
    public string LinkerPath { get; private set; } = string.Empty;

    private IServiceProvider m_services = default!;
    private ILogger<NativeToolchainBuildSdk> m_logger = default!;

    /// <inheritdoc />
    public void Initialize(IServiceProvider services)
    {
        m_services = services;
        m_logger = services.GetRequiredService<ILogger<NativeToolchainBuildSdk>>();

        services.GetSourceRuleRegistry()
            .Register<CompileSourceRule>(CompileSourceRule.Name)
            .Register<ResourceSourceRule>(ResourceSourceRule.Name);
    }

    /// <inheritdoc />
    public IBuildSdkProjectSettings CreateSdkSettings()
    {
        return new NativeToolchainBuildSdkSettings();
    }

    /// <inheritdoc />
    public void ReadSdkSettings(XElement rootElement, IBuildSdkProjectSettings? settings, bool isConfiguration)
    {
        var sdkSettings = (NativeToolchainBuildSdkSettings)settings!;

        var compiler = rootElement.Element("Compiler");
        compiler?.ReadAttribute<CompilerType>("Type", x => sdkSettings.CompilerType = x)
            .ReadAttribute<string>("Arch", x => sdkSettings.CompilerArchitecture = x);

        var msvcSettings = rootElement.Element("MSVCSettings");
        if (msvcSettings != null)
        {
            sdkSettings.MSVCSettings.Deserialize(msvcSettings);
        }

        rootElement.ReadListProperty<string>("IncludePaths", "Path",
                paths => sdkSettings.IncludePaths.AddRange(paths))
            .ReadListProperty<string>("LibrarySearchPaths", "Path",
                paths => sdkSettings.LibrarySearchPaths.AddRange(paths.Select(x =>
                    x.Replace("$(Arch)", sdkSettings.CompilerArchitecture))))
            .ReadListProperty<string>("Libraries", "Lib",
                libs => sdkSettings.Libraries.AddRange(libs));

        rootElement.ReadProperty<WindowsSubsystem>("WindowsSubsystem",
            x => sdkSettings.WindowsSubsystem = x)
            .ReadProperty<string>("WindowsManifest",
                x => sdkSettings.WindowsManifest = x);

        rootElement.ReadProperty<OptimizationLevel>("OptimizationLevel", 
            x => sdkSettings.OptimizationLevel = x)
            .ReadProperty<bool>("EnableLinkTimeOptimization",
                x => sdkSettings.EnableLinkTimeOptimization = x)
            .ReadProperty<WarningLevel>("WarningLevel",
                x => sdkSettings.WarningLevel = x);
    }

    /// <inheritdoc />
    public BuildResult PerformBuild(GameProject project)
    {
        var settings = (NativeToolchainBuildSdkSettings)project.BuildSdkProjectSettings!;

        if (settings.CompilerType == CompilerType.MSVC && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            m_logger.Error("ERROR: MSVC is not available on non-windows operating systems");
            return BuildResult.Failed;
        }

        settings.CompilerProvider = settings.CompilerType switch
        {
            CompilerType.MSVC => MakeMSVCCompiler(settings),
            CompilerType.GCC => throw new NotImplementedException("GCC support is not yet implemented"),
            _ => throw new ArgumentOutOfRangeException(nameof(CompilerType))
        };

        if (settings.CompilerProvider is null)
        {
            m_logger.Error("Failed to locate a valid compiler.");
            if (settings.CompilerType == CompilerType.MSVC)
            {
                m_logger.Error($"Searched for Visual Studio version {settings.MSVCSettings.VisualStudioVersion}");
            }
            return BuildResult.Failed;
        }

        m_logger.Info($"Compiler: {settings.CompilerProvider}");
        m_logger.Debug($"Compiler path: {settings.CompilerProvider.Compiler}");
        m_logger.Debug($"Linker path: {settings.CompilerProvider.Linker}");

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

        dispatcher.RunTask(LinkTask.Generate(project, settings, out var binary));
        if (settings.CompilerType == CompilerType.MSVC)
        {
            dispatcher.RunTask(ManifestToolTask.Generate(project, settings, binary));
        }

        return dispatcher.ExecuteTasks();
    }

    private MSVCCompilerProvider? MakeMSVCCompiler(NativeToolchainBuildSdkSettings settings)
    {
        var install = VSWhereUtility.GetValidVisualStudioInstalls()
            .FirstOrDefault(x 
                => x.InstallationVersion.Matches(settings.MSVCSettings.VisualStudioVersion));

        if (install is null)
            return null;

        var winSdk = VSWhereUtility.GetWindowsSDKInstalls()
            .FirstOrDefault(x => x.Version.Matches(settings.MSVCSettings.WindowsSDKVersion));

        if (winSdk is null)
            m_logger.Warn("No Windows 10 SDK could be located. Compilation is likely to fail.");

        return new MSVCCompilerProvider(settings, install, winSdk);
    }
}