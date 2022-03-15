using System.Xml.Linq;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Sdk;

namespace Brewery.Sdk.NativeToolchain;

/// <summary>
/// Toolchain for building c/c++ code for the host CPU.
/// </summary>
public class NativeToolchainBuildSdk : IBuildSdk
{
    /// <inheritdoc />
    public string Name => "NativeToolchain";

    /// <inheritdoc />
    public void Initialize(IServiceProvider services)
    {
        
    }

    /// <inheritdoc />
    public IBuildSdkProjectSettings? CreateSdkSettings()
    {
        return new NativeToolchainBuildSdkSettings();
    }

    /// <inheritdoc />
    public void ReadSdkSettings(XElement rootElement, IBuildSdkProjectSettings? settings, bool isConfiguration)
    {
        
    }

    /// <inheritdoc />
    public BuildResult PerformBuild(GameProject project)
    {
        return BuildResult.Succeeded;
    }
}