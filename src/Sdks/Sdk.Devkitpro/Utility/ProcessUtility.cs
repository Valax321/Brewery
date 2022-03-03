using System.Diagnostics;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.Utility;

internal static class ProcessUtility
{
    public static BuildResult RunProcess(string processPath, IEnumerable<string> args, out IEnumerable<string> errors)
    {
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
                Console.WriteLine(args.Data);
            };
#endif

        proc.ErrorDataReceived += (sender, args) =>
        {
#if DEBUG
                Console.WriteLine(args.Data);
#endif
            err.Add(args.Data);
        };

        proc.Start();

#if DEBUG
        proc.BeginOutputReadLine();
#endif

        proc.BeginErrorReadLine();

        proc.WaitForExit();

        return proc.ExitCode == 0 ? BuildResult.Succeeded : BuildResult.Failed;
    }
}