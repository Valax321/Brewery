using Brewery.ToolSdk.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Extension methods for getting rule registries from an <see cref="IServiceProvider"/>.
/// </summary>
public static class BuildRuleServiceExtensions
{
    /// <summary>
    /// Gets the source rule registry for the program.
    /// Throws an <see cref="InvalidOperationException"/> if the registry cannot found (this should never happen).
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IRegistry<SourceBuildRule> GetSourceRuleRegistry(this IServiceProvider services)
    {
        return services.GetRequiredService<IRegistry<SourceBuildRule>>();
    }

    /// <summary>
    /// Gets the asset rule registry for the program.
    /// Throws an <see cref="InvalidOperationException"/> if the registry cannot found (this should never happen).
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static IRegistry<AssetBuildRule> GetAssetRuleRegistry(this IServiceProvider services)
    {
        return services.GetRequiredService<IRegistry<AssetBuildRule>>();
    }
}