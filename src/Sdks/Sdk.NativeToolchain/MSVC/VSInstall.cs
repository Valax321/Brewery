using System.Xml.Linq;
using Brewery.ToolSdk.Xml;

namespace Brewery.Sdk.NativeToolchain.MSVC;

internal class VSInstall
{
    public Version InstallationVersion { get; private set; } = new(0, 0, 0);
    public string InstallationPath { get; private set; } = string.Empty;
    public bool IsLaunchable { get; private set; }

    public static VSInstall ParseFromXElement(XElement root)
    {
        var install = new VSInstall();
        root.ReadProperty<string>("installationPath", 
            x => install.InstallationPath = x)
            .ReadProperty<string>("installationVersion",
                x => install.InstallationVersion = Version.Parse(x))
            .ReadProperty<uint>("isLaunchable",
                x => install.IsLaunchable = x > 0);
        return install;
    }

    public Version? GetVCToolsVersion(string? compilerVersion = default)
    {
        var versionStringPath = Path.Combine(InstallationPath, "VC", "Auxiliary", "Build",
            $"Microsoft.VCToolsVersion{(string.IsNullOrEmpty(compilerVersion) ? string.Empty : "." + compilerVersion)}.default.txt");

        if (!File.Exists(versionStringPath))
            return null;

        if (Version.TryParse(File.ReadAllText(versionStringPath).TrimEnd(), out var version))
            return version;

        return null;
    }
}