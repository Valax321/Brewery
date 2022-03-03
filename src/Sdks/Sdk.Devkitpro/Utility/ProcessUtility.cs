using System.Diagnostics;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.Utility;

internal static class ProcessUtility
{
    public static BuildResult RunProcess(string processPath, IEnumerable<string> args, out IEnumerable<string> errors)
    {
        var stdOut = new List<string>();
        var err = new List<string>();
        errors = err;
        var proc = new Process();
        proc.StartInfo = new ProcessStartInfo()
        {
            FileName = processPath,
            Arguments = string.Join(' ', args),
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true
        };

#if DEBUG
            proc.OutputDataReceived += (sender, args) =>
            {
                if (args.Data is null || args.Data.Length == 0)
                    return;

                stdOut.Add(args.Data);
            };
#endif

        proc.ErrorDataReceived += (sender, args) =>
        {
            if (args.Data is null || args.Data.Length == 0)
                return;

            err.Add(args.Data);
        };

        proc.Start();

#if DEBUG
        proc.BeginOutputReadLine();
#endif

        proc.BeginErrorReadLine();

        proc.WaitForExit();

#if DEBUG
        foreach (var line in stdOut)
        {
            Console.WriteLine(line);
        }
#endif

        return proc.ExitCode == 0 ? BuildResult.Succeeded : BuildResult.Failed;
    }
}