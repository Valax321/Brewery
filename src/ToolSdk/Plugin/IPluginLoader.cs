namespace Brewery.ToolSdk.Plugin;

/// <summary>
/// Interface for plugin loader class.
/// </summary>
public interface IPluginLoader
{
    /// <summary>
    /// Loads the plugin at the given path.
    /// </summary>
    /// <param name="pluginPath">The path to the plugin.</param>
    void LoadPlugin(string pluginPath);
}