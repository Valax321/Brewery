using Brewery.ToolSdk.Settings;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ProjectTool.Settings;

internal static class SettingsServiceExtensions
{
    public static IServiceCollection AddEnvironmentSettings(this IServiceCollection collection)
    {
        collection.AddSingleton<IEnvironmentSettings, EnvironmentSettingsRegistry>();
        return collection;
    }
}