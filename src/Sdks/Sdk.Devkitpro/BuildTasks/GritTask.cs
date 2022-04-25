using Brewery.Sdk.DevKitPro.BuildRules;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Utility;

namespace Brewery.Sdk.DevKitPro.BuildTasks;

internal class GritTask : IBuildTask
{
    public Action<string, LogLevel> Log { get; set; } = default!;

    public FileInfo InputFile { get; private set; } = default!;
    public string SymbolName { get; private set; } = string.Empty;
    public FileInfo OutputFile { get; private set; } = default!;
    public SpriteCompileRule.CompressionType Compression { get; private set; }
    public string GritPath { get; private set; } = string.Empty;

    public static GritTask Generate(GameProject project, string imageFile, SpriteCompileRule.CompressionType compression, out FileInfo sourceFile)
    {
        if (project.BuildSdk is not DevKitProBuildSdkBase sdk)
            throw new InvalidOperationException();

        sourceFile = new FileInfo(Path.Combine(project.IntermediateDirectory.FullName, project.AssetsDirectory.Name, imageFile));

        var task = new GritTask()
        {
            InputFile = new FileInfo(Path.Combine(project.AssetsDirectory.FullName, imageFile)),
            OutputFile = sourceFile,
            Compression = compression,
            GritPath = Path.Combine(sdk.DevKitProPath.FullName, "tools", "bin", "grit").Replace('\\', '/'),
            SymbolName = Path.ChangeExtension(imageFile, null).Replace('\\', '/').Replace("/", "__") + "_"
        };

        return task;
    }

    private GritTask()
    { }

    public BuildResult Build()
    {
        Log($"Processing {InputFile.FullName}", LogLevel.Information);

        var flagsFile = Path.ChangeExtension(InputFile.FullName, ".grit.txt")
            .Replace('\\', '/');

        if (OutputFile.Directory is { Exists: false })
            OutputFile.Directory.Create();

        var args = new List<string>()
        {
            InputFile.FullName.Replace('\\', '/'),
            $"-o{OutputFile.FullName.Replace('\\', '/')}",
            $"-s{SymbolName}"
        };

        if (File.Exists(flagsFile))
        {
            args.Add($"-ff{flagsFile}");
        }

        var result = ProcessUtility.RunProcess(GritPath, args, out var errors);
        if (result == 0) 
            return BuildResult.Succeeded;

        foreach (var error in errors)
        {
            Log(error, LogLevel.Error);
        }

        return BuildResult.Failed;
    }
}