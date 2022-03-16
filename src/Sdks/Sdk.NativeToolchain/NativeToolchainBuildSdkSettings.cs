using System.Runtime.InteropServices;
using System.Text;
using Brewery.Sdk.NativeToolchain.MSVC;
using Brewery.ToolSdk.Sdk;

namespace Brewery.Sdk.NativeToolchain;

/// <summary>
/// Sdk-specific settings for the <see cref="NativeToolchainBuildSdk"/>.
/// </summary>
public class NativeToolchainBuildSdkSettings : IBuildSdkProjectSettings
{
    public CompilerType CompilerType { get; internal set; }
    public string CompilerArchitecture { get; internal set; }
    public OptimizationLevel OptimizationLevel { get; internal set; }

    public MSVCSettings MSVCSettings { get; internal set; } = new();
    public WindowsSubsystem WindowsSubsystem { get; internal set; } = WindowsSubsystem.Console;

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