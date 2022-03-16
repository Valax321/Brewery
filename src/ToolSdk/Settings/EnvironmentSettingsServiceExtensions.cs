using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Settings;

/// <summary>
/// Utility methods for getting <see cref="IEnvironmentSettings"/> from service providers.
/// </summary>
public static class EnvironmentSettingsServiceExtensions
{
    /// <summary>
    /// Gets the <see cref="IEnvironmentSettings"/> for the application.
    /// </summary>
    /// <param name="services">Services to get from.</param>
    /// <returns>The <see cref="IEnvironmentSettings"/> instance.</returns>
    public static IEnvironmentSettings GetEnvironmentSettings(this IServiceProvider services)
    {
        return services.GetRequiredService<IEnvironmentSettings>();
    }
}