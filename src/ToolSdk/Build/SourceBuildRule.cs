﻿using System.Xml.Linq;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Xml;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Build rules for source code.
/// </summary>
public abstract class SourceBuildRule : BuildRule
{
    /// <summary>
    /// The language this file is being built for.
    /// </summary>
    public string Language { get; private set; } = string.Empty;

    /// <summary>
    /// Optional override of CPU architecture to build this file for.
    /// Mostly here for GBA/DS where the user would want to distinguish
    /// some rules for building ARMv5 or Thumb instruction sets.
    /// </summary>
    public string Architecture { get; private set; } = string.Empty;

    /// <summary>
    /// Generate a list of build tasks from this rule, given the input directory.
    /// </summary>
    /// <param name="sourceDirectory">The directory to generate tasks from.</param>
    /// <returns>List of build tasks to be executed for this rule.</returns>
    public abstract IEnumerable<IBuildTask> GenerateBuildTasks(GameProject project, DirectoryInfo sourceDirectory, IList<string> alreadyMatchedFiles, out IEnumerable<FileInfo> buildArtifacts);

    /// <inheritdoc />
    public override void Deserialize(XElement element)
    {
        base.Deserialize(element);

        element.ReadAttribute<string>(nameof(Language), value => Language = value, true)
            .ReadAttribute<string>("Arch", value => Architecture = value);
    }
}