using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.NativeToolchain.MSVC;

internal class MSVCCompilerProvider : ICompilerProvider
{
    public string Compiler { get; }
    public string Linker { get; }
    public Version VCToolsVersion { get; }

    public VSInstall VSInstall { get; }

    public WindowsSDKInstall? WindowsSdkInstall { get; }

    public MSVCCompilerProvider(NativeToolchainBuildSdkSettings settings, VSInstall install, WindowsSDKInstall? windowsSdk)
    {
        var toolsVersion = install.GetVCToolsVersion();
        if (toolsVersion == null)
            throw new InvalidOperationException();

        VCToolsVersion = toolsVersion;

        var vcPath = Path.Combine(install.InstallationPath, "VC", "Tools", "MSVC", 
            toolsVersion.ToString(3), "bin", $"Host{RuntimeInformation.ProcessArchitecture.ToString().ToLower()}", settings.CompilerArchitecture);

        Compiler = Path.Combine(vcPath, "cl.exe");
        Linker = Path.Combine(vcPath, "link.exe");

        VSInstall = install;
        WindowsSdkInstall = windowsSdk;
    }

    public IEnumerable<string> BuildCompilerArguments(FileInfo inputFile, FileInfo outputFile, 
        NativeToolchainBuildSdkSettings settings, GameProject project, CompileSourceRule rule)
    {
        var args = new List<string>
        {
            "/nologo",
            "/c"
        };

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        args.Add(settings.OptimizationLevel switch
        {
            OptimizationLevel.O0 => "/Od",
            OptimizationLevel.O1 => "/O1",
            OptimizationLevel.O2 => "/O2",
            OptimizationLevel.O3 => "/Ot",
            OptimizationLevel.Ofast => "/Ot",
            OptimizationLevel.Og => "/Od",
            OptimizationLevel.Os => "/Os",
            OptimizationLevel.Oz => "/Os",
            _ => throw new ArgumentOutOfRangeException(nameof(OptimizationLevel))
        });
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

        args.Add(settings.WarningLevel switch
        {
            WarningLevel.W0 => "/W0",
            WarningLevel.W1 => "/W1",
            WarningLevel.W2 => "/W2",
            WarningLevel.W3 => "/W3",
            WarningLevel.W4 => "/W4",
            WarningLevel.Wall => "/Wall",
            _ => throw new ArgumentOutOfRangeException()
        });

        if (settings.EnableLinkTimeOptimization)
        {
            args.Add("/GL");
        }

        if (settings.EnableExceptions)
        {
            args.Add("/EHsc");
        }

        if (settings.MSVCSettings.EnableDebugging)
        {
            args.Add("/Z7");
        }

        if (!string.IsNullOrEmpty(rule.LanguageStandard))
        {
            args.Add($"/std:{rule.LanguageStandard}");
        }

        args.Add($"/I\"{project.SourceDirectory.FullName}\"");
        args.Add($"/I\"{Path.Combine(VSInstall.InstallationPath, "VC", "Tools", "MSVC", VCToolsVersion.ToString(3), "include")}\"");
        if (WindowsSdkInstall != null)
        {
            // C stdlib
            args.Add($"/I\"{Path.Combine(WindowsSdkInstall.IncludeDirectory, "ucrt")}\"");

            // Windows headers
            args.Add($"/I\"{Path.Combine(WindowsSdkInstall.IncludeDirectory, "um")}\"");

            // More windows headers
            args.Add($"/I\"{Path.Combine(WindowsSdkInstall.IncludeDirectory, "shared")}\"");
        }

        foreach (var path in settings.IncludePaths)
        {
            var ppath = path;
            if (!Path.IsPathRooted(path))
                ppath = Path.Combine(project.ProjectDirectory.FullName, path);

            args.Add($"/I\"{ppath.Replace('/', '\\')}\"");
        }

        args.AddRange(project.DefineSymbols.Select(x => $"/D{Regex.Replace(x, @"\W", string.Empty).ToUpper()}"));
        args.Add($"/D{Regex.Replace(project.Configuration, @"\W", string.Empty).ToUpper()}");

        args.Add($"/Fo\"{outputFile.FullName}\"");

        args.AddRange(new[]
        {
            inputFile.FullName
        });

        return args;
    }

    public IEnumerable<string> BuildLinkerArguments(GameProject project, NativeToolchainBuildSdkSettings settings, FileInfo outputFile)
    {
        // THE LINKER IS SHOUTING FOR SOME REASON
        var args = new List<string>
        {
            "/NOLOGO",
        };

#pragma warning disable CA2208 // Instantiate argument exceptions correctly
        var subsystemName = settings.WindowsSubsystem switch
        {
            WindowsSubsystem.BootApplication => "BOOT_APPLICATION",
            WindowsSubsystem.Console => "CONSOLE",
            WindowsSubsystem.EFIApplication => "EFI_APPLICATION",
            WindowsSubsystem.EFIBootServiceDriver => "EFI_BOOT_SERVICE_DRIVER",
            WindowsSubsystem.EFIRom => "EFI_ROM",
            WindowsSubsystem.EFIRuntimeDriver => "EFI_RUNTIME_DRIVER",
            WindowsSubsystem.Native => "NATIVE",
            WindowsSubsystem.POSIX => "POSIX",
            WindowsSubsystem.Windows => "WINDOWS",
            _ => throw new ArgumentOutOfRangeException(nameof(WindowsSubsystem))
        };
#pragma warning restore CA2208 // Instantiate argument exceptions correctly

        args.Add($"/SUBSYSTEM:{subsystemName}");

        if (settings.EnableLinkTimeOptimization)
        {
            args.Add("/LTCG");
        }

        if (settings.MSVCSettings.EnableIncrementalLinking)
        {
            args.Add("/INCREMENTAL");
        }

        if (settings.MSVCSettings.EnableDebugging)
        {
            args.Add("/DEBUG");
        }

        args.Add($"/OUT:\"{outputFile.FullName}\"");

        if (WindowsSdkInstall != null)
        {
            args.Add($"/LIBPATH:\"{Path.Combine(WindowsSdkInstall.LibraryDirectory, "ucrt", settings.CompilerArchitecture)}\"");
            args.Add($"/LIBPATH:\"{Path.Combine(WindowsSdkInstall.LibraryDirectory, "um", settings.CompilerArchitecture)}\"");
        }
        args.Add($"/LIBPATH:\"{Path.Combine(VSInstall.InstallationPath, "VC", "Tools", "MSVC", VCToolsVersion.ToString(3), "lib", settings.CompilerArchitecture)}\"");

        foreach (var path in settings.LibrarySearchPaths)
        {
            var ppath = path;
            if (!Path.IsPathRooted(path))
                ppath = Path.Combine(project.ProjectDirectory.FullName, path);

            args.Add($"/LIBPATH:\"{ppath.Replace('/', '\\')}\"");
        }

        args.Add("kernel32.lib");
        foreach (var lib in settings.Libraries)
        {
            args.Add('"' + $"{lib}.lib" + '"');
        }

        foreach (var objFile in project.SourceBuildArtifacts)
        {
            args.Add('"' + objFile.FullName + '"');
        }

        return args;
    }

    public string GetExtensionForBinary(GameProject project)
    {
        // TODO: when I implement library building, detect this properly.
        return ".exe";
    }

    public LogLevel ClassifyCompileCommandOutputLine(string compilerMessage, FileInfo fileCompiled)
    {
        if (Regex.Match(compilerMessage, @"\S+\(\d+\): error C\d+").Success)
        {
            return LogLevel.Error;
        }

        if (Regex.Match(compilerMessage, @"\S+\(\d+\): warning [DC]\d+").Success)
        {
            return LogLevel.Warning;
        }

        if (Regex.Match(compilerMessage, @"\S+\(\d+\): note").Success)
        {
            return LogLevel.Warning;
        }

        return LogLevel.Information;
    }

    public LogLevel ClassifyLinkCommandOutputLine(string linkMessage, FileInfo fileLinked)
    {
        if (Regex.Match(linkMessage, @"\S+ : fatal error LNK\d+").Success)
        {
            return LogLevel.Error;
        }

        if (Regex.Match(linkMessage, @"\S+ : error LNK\d+").Success)
        {
            return LogLevel.Error;
        }

        if (Regex.Match(linkMessage, @"\S+ : warning LNK\d+").Success)
        {
            return LogLevel.Warning;
        }

        if (Regex.Match(linkMessage, @"\S+ : note").Success)
        {
            return LogLevel.Warning;
        }

        return LogLevel.Information;
    }

    public override string ToString()
    {
        return $"Visual Studio {VSInstall.InstallationVersion} with VC {VSInstall.GetVCToolsVersion()}";
    }
}