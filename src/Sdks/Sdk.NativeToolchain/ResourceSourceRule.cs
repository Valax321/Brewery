using Brewery.Sdk.NativeToolchain.Tasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.NativeToolchain;

internal class ResourceSourceRule : SourceBuildRule
{
    public const string Name = "Resource";

    public override IEnumerable<IBuildTask> GenerateBuildTasks(GameProject project, DirectoryInfo sourceDirectory, IList<string> alreadyMatchedFiles,
        out IEnumerable<FileInfo> buildArtifacts)
    {
        return GenerateBuildTasksSimple(sourceDirectory, alreadyMatchedFiles,
            (string file, out IEnumerable<FileInfo> artifacts) =>
            {
                var task = ResourceCompilerTask.Generate(project, project.BuildSdkProjectSettings as NativeToolchainBuildSdkSettings ?? throw new InvalidOperationException(), file, out var artifact);
                artifacts = new[] { artifact };
                return task;
            }, out buildArtifacts);
    }
}