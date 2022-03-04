using Brewery.Sdk.DevKitPro;
using Brewery.ToolSdk.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ProjectTool.Plugins;

internal static class PluginServiceExtensions
{
    public static IServiceCollection AddBuiltinPlugins(this IServiceCollection services)
    {
        return services
            .AddSingleton<IPluginLoader, PluginLoader>()
            .AddPlugin<DevKitProPlugin>();
    }

    private static IServiceCollection AddPlugin<TPlugin>(this IServiceCollection services) where TPlugin : class, IPlugin
    {
        return services.AddSingleton<IPlugin, TPlugin>();
    }
}