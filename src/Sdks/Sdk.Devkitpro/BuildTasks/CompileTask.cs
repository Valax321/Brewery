using System.Diagnostics;
using Brewery.Sdk.DevKitPro.BuildRules;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.BuildTasks;

internal class CompileTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; }

    public string SourceFile { get; private set; } = default!;
    public CompileInfo CompileInfo { get; private set; } = default!;

    public static CompileTask Generate(GameProject project, string filePath, SourceCompileRule rule, out FileInfo objectFile)
    {
        if (project.BuildSdk is not DevKitProBuildSdkBase sdk)
            throw new InvalidOperationException();

        var task = new CompileTask
        {
            SourceFile = Path.Combine(project.SourceDirectory.FullName, filePath).Replace('\\', '/'),
            CompileInfo = sdk.GetCompileCommand(project, rule, filePath)
        };

        objectFile = new FileInfo(task.CompileInfo.OutputFile);

        return task;
    }

    public BuildResult Build()
    {
        Log($"Compiling {SourceFile}", LogLevel.Information);

        if (!Directory.Exists(Path.GetDirectoryName(CompileInfo.OutputFile)))
            Directory.CreateDirectory(Path.GetDirectoryName(CompileInfo.OutputFile));

        var proc = new Process();
        proc.StartInfo = new ProcessStartInfo()
        {
            FileName = CompileInfo.CompileCommand[0],
            Arguments = string.Join(' ', CompileInfo.CompileCommand.ToArray()[1..]),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

        Log($"{proc.StartInfo.FileName} {proc.StartInfo.Arguments}", LogLevel.Debug);

        proc.OutputDataReceived += (sender, args) =>
        {
            Console.WriteLine(args.Data);
        };

        proc.ErrorDataReceived += (sender, args) =>
        {
            Console.WriteLine(args.Data);
        };

        proc.Start();

#if DEBUG
        proc.BeginOutputReadLine();
        proc.BeginErrorReadLine();
#endif

        proc.WaitForExit();

        return proc.ExitCode == 0 ? BuildResult.Succeeded : BuildResult.Failed;
    }
}