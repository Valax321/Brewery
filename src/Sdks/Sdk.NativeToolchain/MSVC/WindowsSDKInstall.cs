namespace Brewery.Sdk.NativeToolchain.MSVC;

internal class WindowsSDKInstall
{
    public Version Version { get; init; }
    public string InstallPath { get; init; }

    public string IncludeDirectory => Path.Combine(InstallPath, "Include", Version.ToString());
    public string LibraryDirectory => Path.Combine(InstallPath, "Lib", Version.ToString());
}