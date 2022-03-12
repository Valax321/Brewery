using Brewery.ToolSdk.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Template;

#if ENABLE_EXPERIMENTAL_FEATURES
/// <summary>
/// Utility methods for getting the project template registry from an <see cref="IServiceProvider"/>.
/// </summary>
public static class ProjectTemplateServiceExtensions
{
    /// <summary>
    /// Gets the project template registry from the given <see cref="IServiceProvider"/>.
    /// </summary>
    /// <param name="services">The service provider to get the registry from.</param>
    /// <returns></returns>
    public static IRegistry<ProjectTemplate> GetProjectTemplateRegistry(this IServiceProvider services)
    {
        return services.GetRequiredService<IRegistry<ProjectTemplate>>();
    }
}
#endif