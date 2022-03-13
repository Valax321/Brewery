using System.Diagnostics.CodeAnalysis;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.Sdk.DevKitPro.ARM;

/// <summary>
/// DevKitARM SDK
/// </summary>
public abstract class DevKitProARMBuildSdk : DevKitProBuildSdkBase
{
    /// <inheritdoc />
    public override string CompilerDirectory => "devkitARM";

    /// <inheritdoc />
    public override string CompilerPrefix => "arm-none-eabi-";

    private ILogger<DevKitProARMBuildSdk> m_logger = default!;

    /// <inheritdoc />
    public override void Initialize(IServiceProvider services)
    {
        base.Initialize(services);

        m_logger = services.GetRequiredService<ILogger<DevKitProARMBuildSdk>>();
    }

    /// <inheritdoc />
    public override CompileInfo GetCompileCommand(GameProject project, SourceBuildRule rule, string sourceFile)
    {
        if (project.BuildSdkProjectSettings is not DevKitProBuildSdkProjectSettings settings)
            throw new InvalidOperationException();

        var result = new CompileInfo();

        var compiler = CompilerPrefix + rule.Language switch
        {
            "CXX" or "ObjCXX" => "g++",
            "C" or "ObjC" or "ASM" or _ => "gcc",
        };

        result.OutputFile = Path.Combine(project.IntermediateDirectory.FullName, Path.ChangeExtension(Path.GetRelativePath(project.ProjectDirectory.FullName, sourceFile), ".o"))
            .Replace('\\', '/');

        var flags = new List<string>()
        {
            Path.Combine(DevKitProPath.FullName, CompilerDirectory, "bin", compiler),
            "-save-temps",
            $"-c {sourceFile.Replace('\\', '/')}",
            $"-o {result.OutputFile}"
        };

        AddCpuAndTuneFlags(project, settings, flags);

        // Add architecture-specific flags
        flags.AddRange(rule.Architecture switch
        {
            "Thumb" => new []{ "-mthumb", "-mthumb-interwork" },
            "ARM" or _ => new []{ "-marm" }
        });

        AddOptimizationLevelFlags(project, settings, flags);

        AddPreprocessorDefines(project, settings, flags);

        AddIncludeAndLibraryPaths(project, settings, flags);

        result.CompileCommand = flags;

        return result;
    }

    /// <inheritdoc />
    public override CompileInfo GetLinkCommand(GameProject project, IEnumerable<FileInfo> objectFiles)
    {
        if (project.BuildSdkProjectSettings is not DevKitProBuildSdkProjectSettings settings)
            throw new InvalidOperationException();

        var result = new CompileInfo
        {
            OutputFile = Path.Combine(project.ProjectDirectory.FullName, Path.ChangeExtension(project.OutputName, ".elf"))
                .Replace('\\', '/')
        };

        var flags = new List<string>()
        {
            Path.Combine(DevKitProPath.FullName, CompilerDirectory, "bin", CompilerPrefix + "gcc"),
        };

        // Add architecture-related flags.
        AddLinkerArchFlags(project, settings, flags);
        AddCpuAndTuneFlags(project, settings, flags);
        AddSpecsFlag(project, settings, flags);

        // Add object files
        flags.AddRange(objectFiles.Select(x => x.FullName.Replace('\\', '/')));

        // Add remaining GCC flags
        AddOptimizationLevelFlags(project, settings, flags);
        AddPreprocessorDefines(project, settings, flags);
        AddIncludeAndLibraryPaths(project, settings, flags);

        // Set output file
        flags.Add($"-o {result.OutputFile}");

        result.CompileCommand = flags;

        return result;
    }

    /// <summary>
    /// Add -mcpu and -mtune flags for the given platform.
    /// </summary>
    /// <param name="project">The project being built.</param>
    /// <param name="settings">DKP settings for the project.</param>
    /// <param name="flags">Output flags list.</param>
    protected virtual void AddCpuAndTuneFlags(GameProject project, DevKitProBuildSdkProjectSettings settings,
        List<string> flags)
    { }

    /// <summary>
    /// Add c #defines for the given platform.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="settings"></param>
    /// <param name="flags"></param>
    protected virtual void AddPreprocessorDefines(GameProject project, DevKitProBuildSdkProjectSettings settings,
        List<string> flags)
    {
        flags.AddRange(project.DefineSymbols.Select(x => $"-D{x.ToUpper()}"));
    }

    private void AddOptimizationLevelFlags(GameProject project, DevKitProBuildSdkProjectSettings settings,
        List<string> flags)
    {
        flags.Add($"-O{settings.OptimizationLevel}");
    }

    /// <summary>
    /// Add all flags relating to include paths, library paths and linked libraries.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="settings"></param>
    /// <param name="flags"></param>
    protected virtual void AddIncludeAndLibraryPaths(GameProject project, DevKitProBuildSdkProjectSettings settings,
        List<string> flags)
    {
        flags.Add($"-I{project.SourceDirectory.FullName.Replace('\\', '/')}");
        flags.Add($"-I{Path.Combine(project.IntermediateDirectory.FullName, project.AssetsDirectory.Name).Replace('\\', '/')}");
        if (FindLibrary(project, settings.SystemLib, out var systemLibPath))
        {
            flags.AddRange(GetArgumentsForLibrary(settings.SystemLib, systemLibPath));
        }
        else
        {
            m_logger.Error($"Unable to locate system library {settings.SystemLib}. Searched: {string.Join('\n', settings.LibrarySearchPaths)}");
        }

        foreach (var lib in settings.AdditionalLibs)
        {
            if (FindLibrary(project, lib, out var libPath))
            {
                flags.AddRange(GetArgumentsForLibrary(lib, libPath));
            }
            else
            {
                m_logger.Warn($"Unable to locate additional library {lib}. Searched: {string.Join('\n', settings.LibrarySearchPaths)}");
            }
        }
    }

    /// <summary>
    /// Add linker specs flag.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="settings"></param>
    /// <param name="flags"></param>
    protected virtual void AddSpecsFlag(GameProject project, DevKitProBuildSdkProjectSettings settings,
        List<string> flags)
    { }

    /// <summary>
    /// Add linker-specific arch flags.
    /// </summary>
    /// <param name="project"></param>
    /// <param name="settings"></param>
    /// <param name="flags"></param>
    protected virtual void AddLinkerArchFlags(GameProject project, DevKitProBuildSdkProjectSettings settings,
        List<string> flags)
    { }

    /// <summary>
    /// Redirects a builtin library name to the returned path and static library name.
    /// </summary>
    /// <param name="libraryName">The library to search for redirects.</param>
    /// <returns>A tuple.
    /// Argument 1 is the redirected library path.
    /// Argument 2 is the redirected static library name.
    /// If either value is null, then no redirection should occur.
    /// </returns>
    protected virtual (string?, string?) GetLibraryRedirectedName(string libraryName) => (null, null);

    private bool FindLibrary(GameProject project, string libraryName, [NotNullWhen(true)] out string? libraryPath)
    {
        if (project.BuildSdkProjectSettings is not DevKitProBuildSdkProjectSettings settings)
            throw new InvalidOperationException();

        foreach (var path in settings.LibrarySearchPaths)
        {
            var (pathRedirect, libRedirect) = GetLibraryRedirectedName(libraryName);
            var archivePath = Path.Combine(path, "lib" + (pathRedirect ?? libraryName), "lib",
                "lib" + (libRedirect ?? libraryName) + ".a");

            if (File.Exists(archivePath))
            {
                libraryPath = path;
                return true;
            }
        }

        libraryPath = null;
        return false;
    }

    private IEnumerable<string> GetArgumentsForLibrary(string libraryName, string libraryPath)
    {
        var (pathRedirect, libRedirect) = GetLibraryRedirectedName(libraryName);

        return new[]
        {
            $"-I{Path.Combine(libraryPath, "lib" + (pathRedirect ?? libraryName), "include").Replace('\\', '/')}",
            $"-L{Path.Combine(libraryPath, "lib" + (pathRedirect ?? libraryName), "lib").Replace('\\', '/')}",
            $"-l{libRedirect ?? libraryName}"
        };
    }
}