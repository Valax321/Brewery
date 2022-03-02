using Brewery.Sdk.DevKitPro.BuildTasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.BuildRules;

internal class SourceCompileRule : SourceBuildRule
{
    public const string RuleName = "Compile";

    public override IEnumerable<IBuildTask> GenerateBuildTasks(
        GameProject project, 
        DirectoryInfo sourceDirectory, 
        IList<string> alreadyMatchedFiles, 
        out IEnumerable<FileInfo> buildArtifacts)
    {
        var artifacts = new List<FileInfo>();
        var tasks = new List<IBuildTask>();
        var matchResult = Target.Execute(sourceDirectory);
        foreach (var result in matchResult.Files.Select(x => x.Path).Except(alreadyMatchedFiles))
        {
            tasks.Add(CompileTask.Generate(project, result, this, out var objFile));
            artifacts.Add(objFile);
            alreadyMatchedFiles.Add(result);
        }

        buildArtifacts = artifacts;
        return tasks;
    }
}