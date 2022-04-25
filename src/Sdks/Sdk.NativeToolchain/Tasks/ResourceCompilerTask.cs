using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Brewery.Sdk.NativeToolchain.MSVC;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.NativeToolchain.Tasks;

internal class ResourceCompilerTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public GameProject Project { get; private init; } = default!;
    public NativeToolchainBuildSdkSettings Settings { get; private init; } = default!;
    public MSVCCompilerProvider CompilerProvider { get; private init; } = default!;
    public FileInfo InputFile { get; private init; } = default!;
    public FileInfo OutputFile { get; private init; } = default!;
    public FileInfo RCToolPath { get; private init; } = default!;

    private bool m_skip;

    public static ResourceCompilerTask Generate(GameProject project, NativeToolchainBuildSdkSettings settings,
        string inputFile, out FileInfo objectFile)
    {
        objectFile = new FileInfo(Path.Combine(project.IntermediateDirectory.FullName,
            Path.GetRelativePath(project.ProjectDirectory.FullName, Path.ChangeExtension(inputFile, ".res"))));

        if (settings.CompilerProvider is not MSVCCompilerProvider compilerProvider)
            return new ResourceCompilerTask { m_skip = true };

        if (compilerProvider.WindowsSdkInstall is null)
            return new ResourceCompilerTask { m_skip = true };

        var inFile = new FileInfo(Path.Combine(project.SourceDirectory.FullName, inputFile));

        var task = new ResourceCompilerTask
        {
            Project = project,
            Settings = settings,
            InputFile = inFile,
            OutputFile = objectFile,
            RCToolPath = new FileInfo(Path.Combine(compilerProvider.WindowsSdkInstall.InstallPath, "bin",
                compilerProvider.WindowsSdkInstall.Version.ToString(),
                RuntimeInformation.OSArchitecture.ToString().ToLower(), "rc.exe")),
            CompilerProvider = compilerProvider
        };

        objectFile = task.OutputFile;

        return task;
    }

    public BuildResult Build()
    {
        if (m_skip)
        {
            Log("Cannot compile RC files without a Windows SDK install.", LogLevel.Information);
            return BuildResult.Failed;
        }

        Log($"Compiling resource file {InputFile.FullName}", LogLevel.Information);

        var args = new List<string>
        {
            $"/fo \"{OutputFile.FullName}\""
        };

        args.Add($"/I \"{Project.SourceDirectory.FullName}\"");
        args.Add($"/I \"{Path.Combine(CompilerProvider.VSInstall.InstallationPath, "VC", "Tools", "MSVC", CompilerProvider.VCToolsVersion.ToString(3), "include")}\"");
        if (CompilerProvider.WindowsSdkInstall != null)
        {
            // C stdlib
            args.Add($"/I \"{Path.Combine(CompilerProvider.WindowsSdkInstall.IncludeDirectory, "ucrt")}\"");

            // Windows headers
            args.Add($"/I \"{Path.Combine(CompilerProvider.WindowsSdkInstall.IncludeDirectory, "um")}\"");

            // More windows headers
            args.Add($"/I \"{Path.Combine(CompilerProvider.WindowsSdkInstall.IncludeDirectory, "shared")}\"");
        }

        foreach (var path in Settings.IncludePaths)
        {
            var ppath = path;
            if (!Path.IsPathRooted(path))
                ppath = Path.Combine(Project.ProjectDirectory.FullName, path);

            args.Add($"/I \"{ppath.Replace('/', '\\')}\"");
        }

        args.AddRange(Project.DefineSymbols.Select(x => $"/D{Regex.Replace(x, @"\W", string.Empty).ToUpper()}"));
        args.Add($"/d {Regex.Replace(Project.Configuration, @"\W", string.Empty).ToUpper()}");

        args.Add($"\"{InputFile.FullName}\"");

        var result = ProcessUtility.RunProcess(RCToolPath.FullName, args, out var errors);
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