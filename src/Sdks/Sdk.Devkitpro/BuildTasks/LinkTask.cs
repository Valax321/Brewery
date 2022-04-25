using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.DevKitPro.BuildTasks;

internal class LinkTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public CompileInfo CompileInfo { get; private set; } = default!;

    public static LinkTask Generate(GameProject project, out FileInfo elfFile)
    {
        if (project.BuildSdk is not DevKitProBuildSdkBase sdk)
            throw new InvalidOperationException();

        var task = new LinkTask()
        {
            CompileInfo = sdk.GetLinkCommand(project, project.SourceBuildArtifacts)
        };

        elfFile = new FileInfo(task.CompileInfo.OutputFile);
        return task;
    }

    public BuildResult Build()
    {
        Log($"Linking {CompileInfo.OutputFile}", LogLevel.Information);

        Log(string.Join(' ', CompileInfo.CompileCommand), LogLevel.Debug);

        var fn = CompileInfo.CompileCommand[0];
        var args = CompileInfo.CompileCommand.ToArray()[1..];
        var result = ProcessUtility.RunProcess(fn, args, out var errors);
        if (result == 0)
            return BuildResult.Succeeded;

        foreach (var error in errors)
        {
            Log(error, LogLevel.Error);
        }

        return BuildResult.Failed;
    }
}