using System.Xml.Linq;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Plugin;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Utility;
using Brewery.ToolSdk.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Project;

internal static class GameProjectReader
{
    public static GameProject ReadProject(FileInfo file, IServiceProvider services, string configuration)
    {
        try
        {
            var doc = XDocument.Load(file.OpenRead());
            var proj = new GameProject(file.Directory
                                       ?? throw new GameProjectReadException("Could not determine project directory"), configuration);
            Read(proj, doc.Root ?? throw new GameProjectReadException("Project file had no content"), services);
            return proj;
        }
        catch (Exception ex)
        {
            throw new GameProjectReadException("Exception occurred while loading project", ex);
        }
    }

    private static void Read(GameProject project, XElement rootElement, IServiceProvider services, bool isConfiguration = false)
    {
        rootElement.ReadProperty<string>(nameof(GameProject.SourceDirectory),
                value => project.SourceDirectory = project.ProjectDirectory.GetSubDirectory(value))
            .ReadProperty<string>(nameof(GameProject.AssetsDirectory),
                value => project.AssetsDirectory = project.ProjectDirectory.GetSubDirectory(value))
            .ReadProperty<string>(nameof(GameProject.IntermediateDirectory),
                value => project.IntermediateDirectory = project.ProjectDirectory.GetSubDirectory(Path.Combine(value, project.Configuration)))
            .ReadProperty<string>(nameof(GameProject.OutputName),
                value => project.OutputName = value)
            .ReadProperty<string>(nameof(GameProject.OutputDirectory),
                x => project.OutputDirectory = project.ProjectDirectory.GetSubDirectory(x.Replace("$(Configuration)", project.Configuration)));

        // NOTE: this must come before using any registries otherwise plugin rules will be skipped
        if (!isConfiguration)
        {
            rootElement.ReadListProperty<string>("Plugins", "Plugin", plugins =>
            {
                var pluginLoader = services.GetRequiredService<IPluginLoader>();
                foreach (var plugin in plugins)
                    pluginLoader.LoadPlugin(Path.Combine(project.ProjectDirectory.FullName, plugin));
            });
        }

        if (!isConfiguration)
        {
            var sdkRegistry = services.GetBuildSdkRegistry();

            rootElement.ReadAttribute<string>("Sdk", sdkName =>
            {
                project.BuildSdk = sdkRegistry.GetNamedClass(sdkName);
            }, true);

            if (project.BuildSdk is not null)
            {
                project.BuildSdk.Initialize(services);
                project.BuildSdkProjectSettings = project.BuildSdk.CreateSdkSettings();
            }
        }

        project.BuildSdk?.ReadSdkSettings(rootElement, project.BuildSdkProjectSettings, isConfiguration);

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

        var assetRules = rootElement.Element(nameof(GameProject.AssetRules));
        if (assetRules is not null)
        {
            var registry = services.GetAssetRuleRegistry();

            foreach (var ruleElement in assetRules.Elements())
            {
                var rule = registry.GetNamedClass(ruleElement.Name.LocalName);
                rule?.Deserialize(ruleElement);
                if (rule is not null)
                    project.AssetRules.Add(rule);
            }
        }

        rootElement.ReadListProperty<string>(nameof(GameProject.DefineSymbols), "Define",
            values => project.DefineSymbols.AddRange(values));

        rootElement.ReadListProperty<string>(nameof(GameProject.ExcludeSourceFiles), "Match",
            values => project.ExcludeSourceFiles.AddRange(values));

        if (!isConfiguration)
        {
            var buildConfigurations = rootElement.Element("BuildConfigurations");
            if (buildConfigurations is not null)
            {
                foreach (var configuration in buildConfigurations.Elements("Configuration"))
                {
                    var name = string.Empty;
                    configuration.ReadAttribute<string>("Name", 
                        value => name = value, true);

                    if (name.Equals(project.Configuration))
                    {
                        Read(project, configuration, services, true);
                    }
                }
            }
        }
    }
}