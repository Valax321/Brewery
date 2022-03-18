using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.NativeToolchain;

internal interface ICompilerProvider
{
    public string Compiler { get; }
    public string Linker { get; }

    public IEnumerable<string> BuildCompilerArguments(FileInfo inputFile, FileInfo outputFile,
        NativeToolchainBuildSdkSettings settings, GameProject project, CompileSourceRule rule);

    public IEnumerable<string> BuildLinkerArguments(GameProject project, NativeToolchainBuildSdkSettings settings,
        FileInfo outputFile);

    public string GetExtensionForBinary(GameProject project);
}