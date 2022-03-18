using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Extension methods for setting up build rules.
/// </summary>
public static class BuildRuleExtensions
{
    /// <summary>
    /// Generates a list of build tasks from the project's source rules.
    /// </summary>
    /// <param name="project">The project to generate tasks for.</param>
    /// <param name="buildArtifacts">Collection of build artifacts that will be generated.</param>
    /// <param name="overrideDirectory">Optional override directory to use for gathering source files. The default pulls from the brewproj's SourceDirectory property.</param>
    /// <returns>Collection of build tasks.</returns>
    public static IEnumerable<IBuildTask> GenerateSourceBuildTasks(this GameProject project, 
        out IEnumerable<FileInfo> buildArtifacts, DirectoryInfo? overrideDirectory = null)
    {
        var artifacts = new List<FileInfo>();
        var usedFiles = new List<string>();
        var tasks = new List<IBuildTask>();
        foreach (var rule in project.SourceRules)
        {
            // This is here because the actual source rule doesn't have access to the project until
            // it might be added several times
            project.ExcludeSourceFiles.ForEach(x => rule.Target.AddExclude(x));
            tasks.AddRange(rule.GenerateBuildTasks(project, overrideDirectory ?? project.SourceDirectory, usedFiles, out var arts));
            artifacts.AddRange(arts);
        }

        foreach (var artifact in artifacts)
            artifact.Directory?.Create();

        buildArtifacts = artifacts;
        return tasks;
    }


    /// <summary>
    /// Generates a list of build tasks from the project's asset rules.
    /// </summary>
    /// <param name="project">The project to generate tasks for.</param>
    /// <param name="buildArtifacts">Collection of build artifacts that will be generated.</param>
    /// <param name="overrideDirectory">Optional override directory to use for gathering asset files. The default pulls from the brewproj's AssetDirectory property.</param>
    /// <returns>Collection of build tasks.</returns>
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

        foreach (var artifact in artifacts)
            artifact.Directory?.Create();

        buildArtifacts = artifacts;
        return tasks;
    }
}