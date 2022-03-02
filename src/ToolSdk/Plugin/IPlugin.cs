namespace Brewery.ToolSdk.Plugin;

/// <summary>
/// Interface for plugins to the build system.
/// </summary>
public interface IPlugin
{
    /// <summary>
    /// The display name of the plugin.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Method to register 
    /// </summary>
    void Register(IServiceProvider services);
}