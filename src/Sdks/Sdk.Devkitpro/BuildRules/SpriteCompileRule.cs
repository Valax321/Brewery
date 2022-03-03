using System.Xml.Linq;
using Brewery.Sdk.DevKitPro.BuildTasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Xml;

namespace Brewery.Sdk.DevKitPro.BuildRules;

internal class SpriteCompileRule : AssetBuildRule
{
    public const string RuleName = "Sprite";

    public enum CompressionType
    {
        None,
        LZ77
    }

    public CompressionType Compress { get; private set; } = CompressionType.None;

    public override IEnumerable<IBuildTask> GenerateBuildTasks(
        GameProject project, 
        DirectoryInfo assetDirectory, 
        IList<string> alreadyMatchedFiles,
        out IEnumerable<FileInfo> buildArtifacts)
    {
        var artifacts = new List<FileInfo>();
        var tasks = new List<IBuildTask>();
        var matchResult = Target.Execute(assetDirectory);
        foreach (var result in matchResult.Files.Select(x => x.Path).Except(alreadyMatchedFiles))
        {
            tasks.Add(GritTask.Generate(project, result, Compress, out var sourceFile));
            artifacts.Add(sourceFile);
            alreadyMatchedFiles.Add(result);
        }

        buildArtifacts = artifacts;
        return tasks;
    }

    public override void Deserialize(XElement element)
    {
        base.Deserialize(element);

        element.ReadProperty<string>(nameof(Compress),
            value => Compress = Enum.Parse<CompressionType>(value));
    }
}