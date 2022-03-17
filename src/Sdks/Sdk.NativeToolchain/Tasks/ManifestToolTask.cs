using System.Runtime.InteropServices;
using Brewery.Sdk.NativeToolchain.MSVC;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.NativeToolchain.Tasks;

internal class ManifestToolTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public FileInfo ExecutableFile { get; private init; }
    public FileInfo ManifestToolPath { get; private init; }
    public NativeToolchainBuildSdkSettings Settings { get; private init; }
    public GameProject Project { get; private init; }

    private bool m_skip;

    public static ManifestToolTask Generate(GameProject project, NativeToolchainBuildSdkSettings settings,
        FileInfo exeFile)
    {
        if (settings.CompilerProvider is not MSVCCompilerProvider compilerProvider)
            throw new InvalidOperationException();

        if (compilerProvider.WindowsSdkInstall is null)
            return new ManifestToolTask { m_skip = true };

        var task = new ManifestToolTask
        {
            ExecutableFile = exeFile,
            ManifestToolPath = new FileInfo(Path.Combine(compilerProvider.WindowsSdkInstall.InstallPath, "bin",
                compilerProvider.WindowsSdkInstall.Version.ToString(),
                RuntimeInformation.OSArchitecture.ToString().ToLower(), "mt.exe")),
            Settings = settings,
            Project = project
        };

        return task;
    }

    public BuildResult Build()
    {
        if (m_skip)
        {
            Log("Skipping manifest embedding as a Windows SDK could not be located.", LogLevel.Information);
            return BuildResult.Succeeded;
        }

        var manifestPath = Settings.WindowsManifest;
        if (string.IsNullOrEmpty(manifestPath))
        {
            Log("Skipping manifest embedding: no manifest specified.", LogLevel.Information);
            return BuildResult.Succeeded;
        }

        if (!Path.IsPathRooted(manifestPath))
            manifestPath = Path.Combine(Project.ProjectDirectory.FullName, manifestPath);

        if (!File.Exists(manifestPath))
        {
            Log($"Manifest file {manifestPath} does not exist.", LogLevel.Error);
            return BuildResult.Failed;
        }

        Log($"Embedding manifest {manifestPath}", LogLevel.Information);

        var args = new List<string>
        {
            "-nologo"
        };

        args.Add($"-manifest \"{manifestPath}\"");
        //args.Add($"-inputresource:\"{ExecutableFile.FullName}\";#1");
        args.Add($"-outputresource:\"{ExecutableFile.FullName}\";1");

        Log($"Running {ManifestToolPath.FullName} {string.Join(' ', args)}", LogLevel.Debug);

        var result = ProcessUtility.RunProcess(ManifestToolPath.FullName, args, out var errors);
        if (result == BuildResult.Failed)
        {
            foreach (var error in errors)
                Log(error, LogLevel.Error);
        }

        return result;
    }
}