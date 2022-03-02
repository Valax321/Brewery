namespace Brewery.ToolSdk.Plugin;

/// <summary>
/// Attribute that registers plugins for the assembly it is applied to.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class PluginProviderAttribute : Attribute
{
    /// <summary>
    /// The plugin types provided by this assembly.
    /// </summary>
    public Type[] Plugins { get; }

    /// <summary>
    /// Creates a new <see cref="PluginProviderAttribute"/> instance.
    /// </summary>
    /// <param name="plugins">List of plugin types to register.</param>
    public PluginProviderAttribute(params Type[] plugins)
    {
        Plugins = plugins;
    }
}