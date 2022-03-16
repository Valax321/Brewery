using System.Xml.Linq;
using Brewery.ToolSdk.Xml;

namespace Brewery.Sdk.NativeToolchain.MSVC;

public class MSVCSettings
{
    public string CompilerVersion { get; internal set; }
    public string VisualStudioVersion { get; internal set; } = "*";
    public string WindowsSDKVersion { get; internal set; } = "10.*";

    public bool EnableIncrementalLinking { get; internal set; }
    public bool EnableDebugging { get; internal set; }

    public void Deserialize(XElement element)
    {
        element.ReadProperty<string>(nameof(CompilerVersion),
            x => CompilerVersion = x)
            .ReadProperty<string>(nameof(VisualStudioVersion),
                x => VisualStudioVersion = x)
            .ReadProperty<string>(nameof(WindowsSDKVersion),
                x => WindowsSDKVersion = x)
            .ReadProperty<bool>(nameof(EnableIncrementalLinking),
                x => EnableIncrementalLinking = x)
            .ReadProperty<bool>(nameof(EnableDebugging),
                x => EnableDebugging = x);
    }
}