using Brewery.GBAPluginExample;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Plugin;

[assembly: PluginProvider(typeof(ExampleGBAPlugin))]

namespace Brewery.GBAPluginExample;

public class ExampleGBAPlugin : IPlugin
{
    public string Name => "ExampleGBA";

    public void Register(IServiceProvider services)
    {
        services.GetAssetRuleRegistry()
            .Register<StringsCompilerBuildRule>(StringsCompilerBuildRule.Name);
    }
}
