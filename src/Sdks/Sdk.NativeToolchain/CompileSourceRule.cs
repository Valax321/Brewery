using Brewery.Sdk.NativeToolchain.Tasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.NativeToolchain
{
    internal class CompileSourceRule : SourceBuildRule
    {
        public const string Name = "Compile";

        public override IEnumerable<IBuildTask> GenerateBuildTasks(GameProject project, DirectoryInfo sourceDirectory, IList<string> alreadyMatchedFiles,
            out IEnumerable<FileInfo> buildArtifacts)
        {
            if (project.BuildSdkProjectSettings is not NativeToolchainBuildSdkSettings settings)
                throw new InvalidOperationException();

            return GenerateBuildTasksSimple(sourceDirectory, alreadyMatchedFiles,
                (string file, out IEnumerable<FileInfo> artifacts) =>
                {
                    var task = CompileSourceTask.Generate(project, settings, file, out var artifact);
                    artifacts = new[] { artifact };
                    return task;
                },
                out buildArtifacts);
        }
    }
}
