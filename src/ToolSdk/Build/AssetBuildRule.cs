﻿using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Build rules for assets.
/// </summary>
public abstract class AssetBuildRule : BuildRule
{
    /// <summary>
    /// Generate a list of build tasks for this rule, given the input assets directory.
    /// </summary>
    /// <param name="project">The project file to use.</param>
    /// <param name="assetDirectory">The directory to look for assets in.</param>
    /// <param name="alreadyMatchedFiles">List of files already matched.</param>
    /// <param name="buildArtifacts">Artifacts generated by this build.</param>
    /// <returns>Build tasks generated for the input data.</returns>
    public abstract IEnumerable<IBuildTask> GenerateBuildTasks(
        GameProject project, 
        DirectoryInfo assetDirectory, 
        IList<string> alreadyMatchedFiles, 
        out IEnumerable<FileInfo> buildArtifacts
        );
}