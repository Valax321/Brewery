using Brewery.ProjectTool.Commands;
using Brewery.ProjectTool.Logging;
using Brewery.ProjectTool.Plugins;
using Brewery.ProjectTool.Registry;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Plugin;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ProjectTool;

internal class ToolHost
{
    public static ToolHost Create(IEnumerable<string> args)
    {
        var host = new ToolHost(args);
        return host;
    }

    public IServiceCollection Services { get; }
    private bool ConfigurationFailed { get; set; }

    private ToolHost(IEnumerable<string> args)
    {
        Services = new ServiceCollection()
            .AddLogging()
            .AddRegistries()
            .AddCommands(args, () => ConfigurationFailed = true)
            .AddBuiltinPlugins();
    }

    public void Run()
    {
        if (ConfigurationFailed)
            return;

        using var services = Services.BuildServiceProvider();
        var logger = services.GetRequiredService<ILogger<ToolHost>>();

        foreach (var plugin in services.GetServices<IPlugin>())
        {
            logger.Debug($"Registering built-in plugin {plugin.Name}");
            plugin.Register(services);
        }

        var tool = services.GetService<IToolCommand>();
        if (tool is null)
            throw new CommandCreationException();

        tool.Run();
    }
}