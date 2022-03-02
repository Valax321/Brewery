using Brewery.ToolSdk.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ProjectTool.Logging;

internal static class LoggingServiceExtensions
{
    internal static IServiceCollection AddLogging(this IServiceCollection services)
    {
        services.AddSingleton<ILogProvider, LogConsoleProvider>();
        services.AddSingleton(typeof(ILogger<>), typeof(ToolLogger<>));

        return services;
    }
}