using System.Xml.Linq;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Utility;
using Brewery.ToolSdk.Xml;

namespace Brewery.ToolSdk.Project;

internal static class GameProjectReader
{
    public static GameProject ReadProject(FileInfo file, IServiceProvider services)
    {
        try
        {
            var doc = XDocument.Load(file.OpenRead());
            var proj = new GameProject(file.Directory
                                       ?? throw new GameProjectReadException("Could not determine project directory"));
            Read(proj, doc.Root ?? throw new GameProjectReadException("Project file had no content"), services);
            return proj;
        }
        catch (Exception ex)
        {
            throw new GameProjectReadException("Exception occurred while loading project", ex);
        }
    }

    private static void Read(GameProject project, XElement rootElement, IServiceProvider services)
    {
        rootElement.ReadProperty<string>(nameof(GameProject.SourceDirectory),
                value => project.SourceDirectory = project.ProjectDirectory.GetSubDirectory(value))
            .ReadProperty<string>(nameof(GameProject.AssetsDirectory),
                value => project.AssetsDirectory = project.ProjectDirectory.GetSubDirectory(value))
            .ReadProperty<string>(nameof(GameProject.IntermediateDirectory),
                value => project.IntermediateDirectory = project.ProjectDirectory.GetSubDirectory(value))
            .ReadProperty<string>(nameof(GameProject.OutputName),
                value => project.OutputName = value);

        var sourceRules = rootElement.Element(nameof(GameProject.SourceRules));
        if (sourceRules is not null)
        {
            var registry = services.GetSourceRuleRegistry();

            foreach (var ruleElement in sourceRules.Elements())
            {
                var rule = registry.GetNamedClass(ruleElement.Name.LocalName);
                rule?.Deserialize(ruleElement);
                if (rule is not null)
                    project.SourceRules.Add(rule);
            }
        }

        var sdkRegistry = services.GetBuildSdkRegistry();

        rootElement.ReadAttribute<string>("Sdk", sdkName =>
        {
            project.BuildSdk = sdkRegistry.GetNamedClass(sdkName);
        }, true);

        if (project.BuildSdk is not null)
        {
            project.BuildSdk.Initialize(services);
            project.BuildSdkProjectSettings = project.BuildSdk.ReadSdkSettings(rootElement);
        }
    }
}