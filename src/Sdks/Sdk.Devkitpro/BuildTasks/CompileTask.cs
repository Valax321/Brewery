using Brewery.Sdk.DevKitPro.BuildRules;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.DevKitPro.BuildTasks;

internal class CompileTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public string SourceFile { get; private set; } = default!;
    public CompileInfo CompileInfo { get; private set; } = default!;

    public static CompileTask Generate(GameProject project, string filePath, SourceCompileRule rule, out FileInfo objectFile)
    {
        if (project.BuildSdk is not DevKitProBuildSdkBase sdk)
            throw new InvalidOperationException();

        var task = new CompileTask
        {
            SourceFile = filePath.Replace('\\', '/'),
            CompileInfo = sdk.GetCompileCommand(project, rule, filePath)
        };

        objectFile = new FileInfo(task.CompileInfo.OutputFile);

        return task;
    }

    private CompileTask()
    { }

    public BuildResult Build()
    {
        Log($"Compiling {SourceFile}", LogLevel.Information);

        if (!Directory.Exists(Path.GetDirectoryName(CompileInfo.OutputFile)))
            Directory.CreateDirectory(Path.GetDirectoryName(CompileInfo.OutputFile));

        Log(string.Join(' ', CompileInfo.CompileCommand), LogLevel.Debug);

        var fn = CompileInfo.CompileCommand[0];
        var args = CompileInfo.CompileCommand.ToArray()[1..];
        var result = ProcessUtility.RunProcess(fn, args, out var errors);
        if (result == BuildResult.Succeeded)
            return result;

        foreach (var error in errors)
        {
            Log(error, LogLevel.Error);
        }

        return result;
    }
}