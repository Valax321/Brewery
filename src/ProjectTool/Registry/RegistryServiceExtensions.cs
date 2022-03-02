using Brewery.ToolSdk.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ProjectTool.Registry;

internal static class RegistryServiceExtensions
{
    public static IServiceCollection AddRegistries(this IServiceCollection services)
    {
        return services.AddSingleton(typeof(IRegistry<>), typeof(ClassRegistry<>));
    }
}