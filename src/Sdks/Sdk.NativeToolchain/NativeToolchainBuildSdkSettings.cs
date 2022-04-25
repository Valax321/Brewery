using System.Runtime.InteropServices;
using System.Text;
using Brewery.Sdk.NativeToolchain.MSVC;
using Brewery.ToolSdk.Sdk;

namespace Brewery.Sdk.NativeToolchain;

/// <summary>
/// Sdk-specific settings for the <see cref="NativeToolchainBuildSdk"/>.
/// </summary>
internal class NativeToolchainBuildSdkSettings : IBuildSdkProjectSettings
{
    public CompilerType CompilerType { get; internal set; }
    public string CompilerArchitecture { get; internal set; }
    public OptimizationLevel OptimizationLevel { get; internal set; }
    public WarningLevel WarningLevel { get; internal set; }
    public bool EnableLinkTimeOptimization { get; internal set; }

    public List<string> IncludePaths { get; } = new();
    public List<string> LibrarySearchPaths { get; } = new();
    public List<string> Libraries { get; } = new();

    public MSVCSettings MSVCSettings { get; } = new();
    public WindowsSubsystem WindowsSubsystem { get; internal set; } = WindowsSubsystem.Console;
    public string WindowsManifest { get; internal set; }

    public ICompilerProvider? CompilerProvider { get; internal set; }

    public NativeToolchainBuildSdkSettings()
    {
        CompilerType = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CompilerType.MSVC : CompilerType.GCC;

        CompilerArchitecture = RuntimeInformation.OSArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm => "ARM",
            Architecture.Arm64 => "ARM64",
            _ => throw new PlatformNotSupportedException()
        };
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Compiler Type: {CompilerType}");
        sb.Append($"Compiler Architecture: {CompilerArchitecture}");
        return sb.ToString();
    }
}