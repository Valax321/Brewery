using System.Xml.Linq;
using Brewery.Sdk.NativeToolchain.Tasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Xml;

namespace Brewery.Sdk.NativeToolchain
{
    internal class CompileSourceRule : SourceBuildRule
    {
        public const string Name = "Compile";

        public string LanguageStandard { get; private set; } = string.Empty;

        public override IEnumerable<IBuildTask> GenerateBuildTasks(GameProject project, DirectoryInfo sourceDirectory, IList<string> alreadyMatchedFiles,
            out IEnumerable<FileInfo> buildArtifacts)
        {
            if (project.BuildSdkProjectSettings is not NativeToolchainBuildSdkSettings settings)
                throw new InvalidOperationException();

            return GenerateBuildTasksSimple(sourceDirectory, alreadyMatchedFiles,
                (string file, out IEnumerable<FileInfo> artifacts) =>
                {
                    var task = CompileSourceTask.Generate(this, project, settings, file, out var artifact);
                    artifacts = new[] { artifact };
                    return task;
                },
                out buildArtifacts);
        }

        public override void Deserialize(XElement element)
        {
            base.Deserialize(element);

            element.ReadAttribute<string>(nameof(LanguageStandard), x => LanguageStandard = x);
        }
    }
}
