using Brewery.ToolSdk.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Sdk;

/// <summary>
/// Extensions for getting the <see cref="IRegistry{TClass}"/> for <see cref="IBuildSdk"/>s from a service provider.
/// </summary>
public static class BuildSdkServiceExtensions
{
    /// <summary>
    /// Gets the <see cref="IBuildSdk"/> registry from this service provider.
    /// </summary>
    /// <param name="services">Service provider to look up.</param>
    /// <returns><see cref="IRegistry{TClass}"/> instance from service provider.</returns>
    public static IRegistry<IBuildSdk> GetBuildSdkRegistry(this IServiceProvider services)
    {
        return services.GetRequiredService<IRegistry<IBuildSdk>>();
    }
}