using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;

namespace Brewery.GBAPluginExample;

internal class StringsCompilerBuildRule : AssetBuildRule
{
    public const string Name = "Strings";

    public override IEnumerable<IBuildTask> GenerateBuildTasks(GameProject project, DirectoryInfo assetDirectory, IList<string> alreadyMatchedFiles,
        out IEnumerable<FileInfo> buildArtifacts)
    {
        var artifacts = new List<FileInfo>();
        var tasks = new List<IBuildTask>();
        var matchResult = Target.Execute(assetDirectory);
        foreach (var result in matchResult.Files.Select(x => x.Path).Except(alreadyMatchedFiles))
        {
            tasks.Add(StringsCompilerTask.Generate(project, result, out var sourceFile));
            artifacts.Add(sourceFile);
            alreadyMatchedFiles.Add(result);
        }

        buildArtifacts = artifacts;
        return tasks;
    }
}