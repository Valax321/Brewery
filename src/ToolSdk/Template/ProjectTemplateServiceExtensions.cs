using Brewery.ToolSdk.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Template;

public static class ProjectTemplateServiceExtensions
{
    public static IRegistry<ProjectTemplate> GetProjectTemplateRegistry(this IServiceProvider services)
    {
        return services.GetRequiredService<IRegistry<ProjectTemplate>>();
    }
}