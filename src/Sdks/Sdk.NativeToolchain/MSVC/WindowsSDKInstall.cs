namespace Brewery.Sdk.NativeToolchain.MSVC;

internal class WindowsSDKInstall
{
    public Version Version { get; init; } = default!;
    public string InstallPath { get; init; } = string.Empty;

    public string IncludeDirectory => Path.Combine(InstallPath, "Include", Version.ToString());
    public string LibraryDirectory => Path.Combine(InstallPath, "Lib", Version.ToString());
}