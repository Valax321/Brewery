﻿using Brewery.Sdk.DevKitPro;
using Brewery.Sdk.DevKitPro.ARM;
using Brewery.Sdk.DevKitPro.BuildRules;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Plugin;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Template;

// Register the plugin type
// This goes unused at the moment as built-in plugins
// are manually registered in ProjectTool but
// it's here for consistency with user plugins
[assembly: PluginProvider(typeof(DevKitProPlugin))]

namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// Plugin that provides support for Devkitpro builds.
/// </summary>
public class DevKitProPlugin : IPlugin
{
    public string Name => "Devkitpro";

    public void Register(IServiceProvider services)
    {
        services.GetBuildSdkRegistry()
            .Register<DevKitProGBABuildSdk>(DevKitProGBABuildSdk.SdkName);

        services.GetProjectTemplateRegistry()
            .Register<DevKitProProjectTemplate>(DevKitProProjectTemplate.TemplateName);

        services.GetSourceRuleRegistry()
            .Register<SourceCompileRule>(SourceCompileRule.RuleName);

        services.GetAssetRuleRegistry()
            .Register<SpriteCompileRule>(SpriteCompileRule.RuleName);
    }
}