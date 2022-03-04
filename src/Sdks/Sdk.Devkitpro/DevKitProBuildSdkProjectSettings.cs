using System.Text;
using Brewery.ToolSdk.Sdk;

namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// DevKitPro-specific SDK project settings.
/// </summary>
public class DevKitProBuildSdkProjectSettings : IBuildSdkProjectSettings
{
    /// <summary>
    /// System library that should be linked against.
    /// Some platforms have several options, such as the GBA having tonc and libgba.
    /// </summary>
    public string SystemLib { get; set; } = string.Empty;

    /// <summary>
    /// Additional libraries that should be linked. The DevKitPro SDK root path is searched for these.
    /// </summary>
    public List<string> AdditionalLibs { get; set; } = new();

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"System Library: {SystemLib}");
        sb.AppendLine($"Additional Libraries: {string.Join(", ", AdditionalLibs)}");
        return sb.ToString();
    }
}