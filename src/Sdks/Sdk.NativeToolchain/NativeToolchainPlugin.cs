using Brewery.ToolSdk.Plugin;

namespace Brewery.Sdk.NativeToolchain;

/// <summary>
/// Plugin interface for the <see cref="NativeToolchainBuildSdk"/>.
/// </summary>
internal class NativeToolchainPlugin : IPlugin
{
    /// <inheritdoc />
    public string Name => "NativeToolchain";

    /// <inheritdoc />
    public void Register(IServiceProvider services)
    {
        
    }
}