namespace Brewery.ToolSdk.Utility;

/// <summary>
/// Utility methods for the <see cref="DirectoryInfo"/> class.
/// </summary>
public static class DirectoryInfoExtensions
{
    /// <summary>
    /// Gets a <see cref="DirectoryInfo"/> representing the sub-directory of the provided <see cref="DirectoryInfo"/>.
    /// </summary>
    /// <param name="directory">The parent directory.</param>
    /// <param name="subDirectory">Path to the sub-directory.</param>
    /// <returns></returns>
    public static DirectoryInfo GetSubDirectory(this DirectoryInfo directory, string subDirectory)
    {
        var invalidSeparator = Path.PathSeparator == '/' ? '\\' : '/';
        var path = Path.Combine(directory.FullName, subDirectory.Replace(invalidSeparator, Path.DirectorySeparatorChar));
        return new DirectoryInfo(path);
    }
}