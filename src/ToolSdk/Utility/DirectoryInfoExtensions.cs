namespace Brewery.ToolSdk.Utility;

public static class DirectoryInfoExtensions
{
    public static DirectoryInfo GetSubDirectory(this DirectoryInfo directory, string subDirectory)
    {
        var invalidSeparator = Path.PathSeparator == '/' ? '\\' : '/';
        var path = Path.Combine(directory.FullName, subDirectory.Replace(invalidSeparator, Path.DirectorySeparatorChar));
        return new DirectoryInfo(path);
    }
}