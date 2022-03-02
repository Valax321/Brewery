using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Build;

public static class SourceRulesExtensions
{
    public static IEnumerable<IBuildTask> GenerateSourceBuildTasks(this GameProject project, out IEnumerable<FileInfo> buildArtifacts)
    {
        var artifacts = new List<FileInfo>();
        var usedFiles = new List<string>();
        var tasks = new List<IBuildTask>();
        foreach (var rule in project.SourceRules)
        {
            tasks.AddRange(rule.GenerateBuildTasks(project, project.SourceDirectory, usedFiles, out var arts));
            artifacts.AddRange(arts);
        }

        buildArtifacts = artifacts;
        return tasks;
    }
}