using System.Reflection;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Plugin;

namespace Brewery.ProjectTool.Plugins;

internal class PluginLoader : IPluginLoader
{
    private readonly ILogger<PluginLoader> m_logger;
    private readonly IServiceProvider m_services;

    public PluginLoader(ILogger<PluginLoader> logger, IServiceProvider services)
    {
        m_logger = logger;
        m_services = services;
    }

    public void LoadPlugin(string pluginPath)
    {
        if (Path.GetExtension(pluginPath) == ".dll" && File.Exists(pluginPath))
        {
            try
            {
                var pluginAssembly = Assembly.LoadFile(pluginPath);
                foreach (var attribute in pluginAssembly.GetCustomAttributes<PluginProviderAttribute>())
                {
                    foreach (var pluginType in attribute.Plugins)
                    {
                        if (Activator.CreateInstance(pluginType) is IPlugin plugin)
                        {
                            m_logger.Info($"Loaded plugin {plugin.Name}");
                            plugin.Register(m_services);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                m_logger.Error($"Could not load plugin: {ex.Message}");
            }
        }
    }
}