using System.Diagnostics;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.BuildTasks
{
    internal class GBAFixTask : IBuildTask
    {
        public Action<string, LogLevel> Log { get; set; }

        public IReadOnlyList<string> CopyCommand { get; set; }
        public IReadOnlyList<string> Command { get; private set; }
        public string WorkingDirectory { get; private set; }
        public FileInfo ElfFile { get; private set; }
        public FileInfo GBAFile { get; private set; }

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

        public BuildResult Build()
        {
            Log($"Fixing GBA file {Command[1]}", LogLevel.Information);

            var result = ObjcopyCommand();
            if (result == BuildResult.Failed)
                return result;

            return GBAFixCommand();
        }

        private BuildResult ObjcopyCommand()
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = CopyCommand[0],
                Arguments = string.Join(' ', CopyCommand.ToArray()[1..]),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = WorkingDirectory
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

        private BuildResult GBAFixCommand()
        {
            var proc = new Process();
            proc.StartInfo = new ProcessStartInfo()
            {
                FileName = Command[0],
                Arguments = string.Join(' ', Command.ToArray()[1..]),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = WorkingDirectory
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
}
