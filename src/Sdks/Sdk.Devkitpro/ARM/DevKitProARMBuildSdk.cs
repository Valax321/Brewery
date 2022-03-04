using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;

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
            $"-o {result.OutputFile}",
            "-mcpu=arm7tdmi",
            "-mtune=arm7tdmi"
        };

        // Add architecture-specific flags
        flags.AddRange(rule.Architecture switch
        {
            "Thumb" => new []{ "-mthumb", "-mthumb-interwork" },
            "ARM" or _ => new []{ "-marm" }
        });

        if (project.Configuration == "Debug")
        {
            flags.Add("-O0");
        }
        else
        {
            flags.Add("-O3");
        }

        flags.AddRange(project.DefineSymbols.Select(x => $"-D{x.ToUpper()}"));

        flags.Add($"-I{project.SourceDirectory.FullName.Replace('\\', '/')}");
        flags.Add($"-I{Path.Combine(project.IntermediateDirectory.FullName, project.AssetsDirectory.Name).Replace('\\', '/')}");
        flags.AddRange(GetArgumentsForLibrary(settings.SystemLib));
        foreach (var lib in settings.AdditionalLibs)
        {
            flags.AddRange(GetArgumentsForLibrary(lib));
        }

        result.CompileCommand = flags;

        return result;
    }

    private IEnumerable<string> GetArgumentsForLibrary(string libraryName)
    {
        var (pathRedirect, libRedirect) = GetLibraryRedirectedName(libraryName);

        return new[]
        {
            $"-I{Path.Combine(DevKitProPath.FullName, "lib" + (pathRedirect ?? libraryName), "include").Replace('\\', '/')}",
            $"-L{Path.Combine(DevKitProPath.FullName, "lib" + (pathRedirect ?? libraryName), "lib").Replace('\\', '/')}",
            $"-l{libRedirect ?? libraryName}"
        };
    }

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

    /// <inheritdoc />
    public override CompileInfo GetLinkCommand(GameProject project, IEnumerable<FileInfo> objectFiles)
    {
        if (project.BuildSdkProjectSettings is not DevKitProBuildSdkProjectSettings settings)
            throw new InvalidOperationException();

        var result = new CompileInfo();

        result.OutputFile = Path.Combine(project.ProjectDirectory.FullName, Path.ChangeExtension(project.OutputName, ".elf"))
            .Replace('\\', '/');

        var flags = new List<string>()
        {
            Path.Combine(DevKitProPath.FullName, CompilerDirectory, "bin", CompilerPrefix + "gcc"),
            "-mthumb", 
            "-mthumb-interwork",
            "-mcpu=arm7tdmi",
            "-mtune=arm7tdmi",
            "-specs=gba.specs",
        };

        flags.AddRange(objectFiles.Select(x => x.FullName.Replace('\\', '/')));

        if (project.Configuration == "Debug")
        {
            flags.Add("-O0");
        }
        else
        {
            flags.Add("-O3");
        }

        flags.AddRange(project.DefineSymbols.Select(x => $"-D{x.ToUpper()}"));

        flags.Add($"-I{project.SourceDirectory.FullName.Replace('\\', '/')}");
        flags.Add($"-I{Path.Combine(project.IntermediateDirectory.FullName, project.AssetsDirectory.Name).Replace('\\', '/')}");
        flags.AddRange(GetArgumentsForLibrary(settings.SystemLib));
        foreach (var lib in settings.AdditionalLibs)
        {
            flags.AddRange(GetArgumentsForLibrary(lib));
        }

        flags.Add($"-o {result.OutputFile}");

        result.CompileCommand = flags;

        return result;
    }
}