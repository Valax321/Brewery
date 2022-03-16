
using System.Xml.Linq;
using Brewery.ToolSdk.Xml;

namespace Brewery.Sdk.NativeToolchain.MSVC;

public class MSVCSettings : IXmlDeserializable
{
    public string CompilerVersion { get; internal set; }
    public string VisualStudioVersion { get; internal set; } = "*";
    public string WindowsSDKVersion { get; internal set; } = "10.*";

    public void Deserialize(XElement element)
    {
        element.ReadProperty<string>("CompilerVersion",
            x => CompilerVersion = x)
            .ReadProperty<string>("VisualStudioVersion",
                x => VisualStudioVersion = x)
            .ReadProperty<string>("WindowsSDKVersion",
                x => WindowsSDKVersion = x);
    }
}