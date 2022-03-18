using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.NativeToolchain.Tasks;

internal class CompileSourceTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public string CompileCommand { get; private set; } = default!;
    public FileInfo InputFile { get; private set; } = default!;
    public FileInfo ObjectFile { get; private set; } = default!;
    public GameProject Project { get; private set; } = default!;
    public NativeToolchainBuildSdkSettings BuildSettings { get; private set; } = default!;
    public CompileSourceRule Rule { get; private set; } = default!;

    private CompileSourceTask()
    { }

    public static CompileSourceTask Generate(CompileSourceRule rule, GameProject project, NativeToolchainBuildSdkSettings settings,
        string inputFile, out FileInfo buildArtifact)
    {
        var task = new CompileSourceTask
        {
            Project = project,
            BuildSettings = settings,
            CompileCommand = settings.CompilerProvider!.Compiler,
            InputFile = new FileInfo(inputFile),
            ObjectFile = new FileInfo(Path.Combine(project.IntermediateDirectory.FullName,
                Path.GetRelativePath(project.ProjectDirectory.FullName, Path.ChangeExtension(inputFile, ".obj")))),
            Rule = rule
        };

        buildArtifact = task.ObjectFile;

        return task;
    }

    public BuildResult Build()
    {
        Log($"Compiling {InputFile.FullName}", LogLevel.Information);

        var args = BuildSettings.CompilerProvider!
            .BuildCompilerArguments(InputFile, ObjectFile, BuildSettings, Project, Rule);

        Log($"Command: {CompileCommand} {string.Join(' ', args)}", LogLevel.Debug);

        var result = ProcessUtility.RunProcess(CompileCommand, args, out var errors);
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