namespace Brewery.Sdk.NativeToolchain;

internal static class VersionUtility
{
    public static bool Matches(this Version installedVersion, string allowedVersions)
    {
        var components = new[] { int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue };
        var split = allowedVersions.Split('.');

        for (int i = 0; i < split.Length; ++i)
            if (split[i] != "*")
                components[i] = int.Parse(split[i]);

        return installedVersion <= new Version(components[0], components[1], components[2], components[3]);
    }
}