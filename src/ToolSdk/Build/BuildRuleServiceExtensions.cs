using Brewery.ToolSdk.Registry;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ToolSdk.Build;

public static class BuildRuleServiceExtensions
{
    public static IRegistry<SourceBuildRule> GetSourceRuleRegistry(this IServiceProvider services)
    {
        return services.GetRequiredService<IRegistry<SourceBuildRule>>();
    }
}