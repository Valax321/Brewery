using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Build;

public static class BuildRuleExtensions
{
    public static IEnumerable<IBuildTask> GenerateSourceBuildTasks(this GameProject project, 
        out IEnumerable<FileInfo> buildArtifacts, DirectoryInfo? overrideDirectory = null)
    {
        var artifacts = new List<FileInfo>();
        var usedFiles = new List<string>();
        var tasks = new List<IBuildTask>();
        foreach (var rule in project.SourceRules)
        {
            tasks.AddRange(rule.GenerateBuildTasks(project, overrideDirectory ?? project.SourceDirectory, usedFiles, out var arts));
            artifacts.AddRange(arts);
        }

        buildArtifacts = artifacts;
        return tasks;
    }

    public static IEnumerable<IBuildTask> GenerateAssetBuildTasks(this GameProject project,
        out IEnumerable<FileInfo> buildArtifacts, DirectoryInfo? overrideDirectory = null)
    {
        var artifacts = new List<FileInfo>();
        var usedFiles = new List<string>();
        var tasks = new List<IBuildTask>();
        foreach (var rule in project.AssetRules)
        {
            tasks.AddRange(rule.GenerateBuildTasks(project, overrideDirectory ?? project.AssetsDirectory, usedFiles, out var arts));
            artifacts.AddRange(arts);
        }

        buildArtifacts = artifacts;
        return tasks;
    }
}