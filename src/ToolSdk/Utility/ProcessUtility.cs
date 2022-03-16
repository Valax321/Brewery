using System.Diagnostics;
using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Utility;

/// <summary>
/// Utility methods for running a process and getting the result.
/// </summary>
public static class ProcessUtility
{
    /// <summary>
    /// Runs the given process, and returns whether the build was successful.
    /// </summary>
    /// <param name="processPath">The process to run.</param>
    /// <param name="args">Arguments passed to the process.</param>
    /// <param name="errors">List of lines outputted to stderr during execution.</param>
    /// <returns><see cref="BuildResult.Succeeded"/> if the exit code was 0, otherwise <see cref="BuildResult.Failed"/>.</returns>
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
            proc.OutputDataReceived += (_, args) =>
            {
                if (args.Data is null || args.Data.Length == 0)
                    return;

                stdOut.Add(args.Data);
            };
#endif

        proc.ErrorDataReceived += (_, args) =>
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