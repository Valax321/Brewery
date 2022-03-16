using System.Runtime.InteropServices;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.NativeToolchain.MSVC;

internal class MSVCCompilerProvider : ICompilerProvider
{
    public string Compiler { get; }
    public string Linker { get; }
    public Version VCToolsVersion { get; }

    private readonly VSInstall m_vsInstall;
    private readonly WindowsSDKInstall? m_winSdk;

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

        m_vsInstall = install;
        m_winSdk = windowsSdk;
    }

    public IEnumerable<string> BuildCompilerArguments(FileInfo inputFile, FileInfo outputFile, 
        NativeToolchainBuildSdkSettings settings, GameProject project)
    {
        var args = new List<string>
        {
            "/nologo",
            "/c"
        };

        args.Add(settings.OptimizationLevel switch
        {
            OptimizationLevel.O0 => "/Od",
            OptimizationLevel.O1 => "/O1",
            OptimizationLevel.O2 => "/O2",
            OptimizationLevel.O3 => "/Ot",
            OptimizationLevel.Ofast => "/Ot",
            OptimizationLevel.Og => "/Od",
            OptimizationLevel.Os => "/Os",
            OptimizationLevel.Oz => "/Os"
        });

        args.Add($"/I\"{project.SourceDirectory.FullName}\"");
        args.Add($"/I\"{Path.Combine(m_vsInstall.InstallationPath, "VC", "Tools", "MSVC", VCToolsVersion.ToString(3), "include")}\"");
        if (m_winSdk != null)
        {
            // C stdlib
            args.Add($"/I\"{Path.Combine(m_winSdk.IncludeDirectory, "ucrt")}\"");

            // Windows headers
            args.Add($"/I\"{Path.Combine(m_winSdk.IncludeDirectory, "um")}\"");
        }

        args.AddRange(project.DefineSymbols.Select(x => $"/D{x.ToUpper()}"));

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
        args.Add($"/SUBSYSTEM:{settings.WindowsSubsystem}");

        // TODO: does this affect the optimization of the binary?
        // I'm just enabling it so we always get a PDB
        args.Add("/PDB");

        args.Add($"/OUT:\"{outputFile.FullName}\"");

        args.Add($"/LIBPATH:\"{Path.Combine(m_winSdk.LibraryDirectory, "ucrt", settings.CompilerArchitecture)}\"");
        args.Add($"/LIBPATH:\"{Path.Combine(m_winSdk.LibraryDirectory, "um", settings.CompilerArchitecture)}\"");
        args.Add($"/LIBPATH:\"{Path.Combine(m_vsInstall.InstallationPath, "VC", "Tools", "MSVC", VCToolsVersion.ToString(3), "lib", settings.CompilerArchitecture)}\"");
        args.Add('"' + "kernel32.lib" + '"');

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

    public override string ToString()
    {
        return $"Visual Studio {m_vsInstall.InstallationVersion} with VC {m_vsInstall.GetVCToolsVersion()}";
    }
}