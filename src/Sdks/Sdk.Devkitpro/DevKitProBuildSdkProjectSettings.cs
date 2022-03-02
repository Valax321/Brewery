using System.Text;
using Brewery.ToolSdk.Sdk;

namespace Brewery.Sdk.DevKitPro;

public class DevKitProBuildSdkProjectSettings : IBuildSdkProjectSettings
{
    public string SystemLib { get; set; } = string.Empty;
    public List<string> AdditionalLibs { get; set; } = new();

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"System Library: {SystemLib}");
        sb.AppendLine($"Additional Libraries: {string.Join(", ", AdditionalLibs)}");
        return sb.ToString();
    }
}