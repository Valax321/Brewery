using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.DevKitPro.BuildTasks;

internal class GBAFixTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public IReadOnlyList<string> CopyCommand { get; set; } = default!;
    public IReadOnlyList<string> Command { get; private set; } = default!;
    public string WorkingDirectory { get; private set; } = default!;
    public FileInfo ElfFile { get; private set; } = default!;
    public FileInfo GBAFile { get; private set; } = default!;

    public static GBAFixTask Generate(GameProject project, FileInfo elfFile, out FileInfo gbaFile)
    {
        if (project.BuildSdk is not DevKitProBuildSdkBase sdk)
            throw new InvalidOperationException();

        var outputFile = Path.ChangeExtension(elfFile.FullName, ".gba").Replace('\\', '/');

        var copyCommands = new List<string>()
        {
            Path.Combine(sdk.DevKitProPath.FullName, sdk.CompilerDirectory, "bin", sdk.CompilerPrefix + "objcopy"),
            "-O binary",
            elfFile.FullName.Replace('\\', '/'),
            outputFile
        };

        var commands = new List<string>()
        {
            Path.Combine(sdk.DevKitProPath.FullName, "tools", "bin", "gbafix").Replace('\\', '/'),
            outputFile
        };

        var task = new GBAFixTask()
        {
            WorkingDirectory = project.ProjectDirectory.FullName,
            Command = commands,
            CopyCommand = copyCommands,
            GBAFile = new FileInfo(outputFile),
            ElfFile = elfFile
        };

        gbaFile = task.GBAFile;
        return task;
    }

    private GBAFixTask()
    { }

    public BuildResult Build()
    {
        Log($"Making GBA ROM {Command[1]}", LogLevel.Information);

        var result = ObjcopyCommand();
        if (result == BuildResult.Failed)
            return result;

        return GBAFixCommand();
    }

    private BuildResult ObjcopyCommand()
    {
        var fn = CopyCommand[0];
        var args = CopyCommand.ToArray()[1..];

        var result = ProcessUtility.RunProcess(fn, args, out var errors);
        if (result == BuildResult.Succeeded)
            return result;

        foreach (var error in errors)
        {
            Log(error, LogLevel.Error);
        }

        return result;
    }

    private BuildResult GBAFixCommand()
    {
        var fn = Command[0];
        var args = Command.ToArray()[1..];

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