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
    /// <param name="project">The project to generate tasks for.</param>
    /// <param name="sourceDirectory">The directory to generate tasks from.</param>
    /// <param name="alreadyMatchedFiles">List of files that have already matched source rules.</param>
    /// <param name="buildArtifacts">Collection of artifacts generated by this rule.</param>
    /// <returns>List of build tasks to be executed for this rule.</returns>
    public abstract IEnumerable<IBuildTask> GenerateBuildTasks(GameProject project, DirectoryInfo sourceDirectory, IList<string> alreadyMatchedFiles, out IEnumerable<FileInfo> buildArtifacts);

    /// <summary>
    /// Delegate signature for a simple task generator.
    /// </summary>
    /// <param name="inputFile">Full path to the matched file.</param>
    /// <param name="buildArtifacts">Output list of build artifacts generated.</param>
    /// <returns></returns>
    protected delegate IBuildTask SimpleTaskGeneratorFunc(string inputFile, out IEnumerable<FileInfo> buildArtifacts);

    /// <summary>
    /// A simple implementation of <see cref="GenerateBuildTasks"/> that handles matching for you.
    /// Simply plug in a <see cref="SimpleTaskGeneratorFunc"/> method that returns a <see cref="IBuildTask"/> and a list
    /// of build artifacts.
    /// </summary>
    /// <param name="sourceDirectory">The directory to generate tasks from.</param>
    /// <param name="alreadyMatchedFiles">List of files that have already matched source rules.</param>
    /// <param name="taskGenerator"><see cref="SimpleTaskGeneratorFunc"/> that generates tasks for matched files.</param>
    /// <param name="buildArtifacts">Collection of artifacts generated by this rule.</param>
    /// <returns>List of build tasks to be executed for this rule.</returns>
    protected IEnumerable<IBuildTask> GenerateBuildTasksSimple(DirectoryInfo sourceDirectory,
        IList<string> alreadyMatchedFiles, SimpleTaskGeneratorFunc taskGenerator, out IEnumerable<FileInfo> buildArtifacts)
    {
        var artifacts = new List<FileInfo>();
        var tasks = new List<IBuildTask>();
        var matchResult = Target.Execute(sourceDirectory);
        foreach (var result in matchResult.Files.Select(x => x.Path).Except(alreadyMatchedFiles))
        {
            tasks.Add(taskGenerator(Path.Combine(sourceDirectory.FullName, result), out var a));
            artifacts.AddRange(a);
            alreadyMatchedFiles.Add(result);
        }

        buildArtifacts = artifacts;
        return tasks;
    }

    /// <inheritdoc />
    public override void Deserialize(XElement element)
    {
        base.Deserialize(element);

        element.ReadAttribute<string>(nameof(Language), value => Language = value, true)
            .ReadAttribute<string>("Arch", value => Architecture = value);
    }
}