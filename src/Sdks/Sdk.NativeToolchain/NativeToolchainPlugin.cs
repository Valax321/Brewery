using Brewery.Sdk.NativeToolchain;
using Brewery.ToolSdk.Plugin;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Settings;

[assembly: PluginProvider(typeof(NativeToolchainPlugin))]

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
        services.GetBuildSdkRegistry()
            .Register<NativeToolchainBuildSdk>(NativeToolchainBuildSdk.SdkName);

        services.GetEnvironmentSettings()
            .RegisterSetting(Name, "MinGWPath", string.Empty);
    }
}