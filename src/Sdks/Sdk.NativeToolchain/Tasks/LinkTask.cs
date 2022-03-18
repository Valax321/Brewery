using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.NativeToolchain.Tasks;

internal class LinkTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public string LinkCommand { get; private set; } = default!;
    public NativeToolchainBuildSdkSettings BuildSettings { get; private set; } = default!;
    public FileInfo BinaryFile { get; private set; } = default!;
    public GameProject Project { get; private set; } = default!;

    private LinkTask()
    { }

    public static LinkTask Generate(GameProject project, NativeToolchainBuildSdkSettings settings, out FileInfo binary)
    {
        var task = new LinkTask
        {
            Project = project,
            BuildSettings = settings,
            BinaryFile = new FileInfo(Path.Combine(project.OutputDirectory.FullName,
                project.OutputName + settings.CompilerProvider!.GetExtensionForBinary(project))),
            LinkCommand = settings.CompilerProvider!.Linker
        };

        binary = task.BinaryFile;

        return task;
    }

    public BuildResult Build()
    {
        Log($"Linking {BinaryFile.FullName}", LogLevel.Information);

        BinaryFile.Directory?.Create();

        var args = BuildSettings.CompilerProvider!
            .BuildLinkerArguments(Project, BuildSettings, BinaryFile);

        Log($"Command: {LinkCommand} {string.Join(' ', args)}", LogLevel.Debug);

        var result = ProcessUtility.RunProcess(LinkCommand, args, out var errors);
        if (result == BuildResult.Failed)
        {
            foreach (var error in errors)
            {
                Log(error, LogLevel.Error);
            }
        }

        return result;
    }
}