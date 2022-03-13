using System.Reflection;
using System.Runtime.Loader;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Plugin;

namespace Brewery.ProjectTool.Plugins;

internal class PluginLoader : IPluginLoader
{
    private readonly ILogger<PluginLoader> m_logger;
    private readonly IServiceProvider m_services;

    private readonly Dictionary<string, BreweryPluginLoadContext> m_pluginContexts = new();

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
                if (!m_pluginContexts.TryGetValue(pluginPath, out var pluginContext))
                {
                    pluginContext = new BreweryPluginLoadContext(pluginPath);
                    m_pluginContexts.Add(pluginPath, pluginContext);

                    m_logger.Debug($"Created plugin context {pluginPath}");
                }

                var pluginAssembly = pluginContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginPath)));
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