using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Wrapper extension methods around <see cref="Matcher"/> so referencing libraries don't need to
/// reference <see cref="Microsoft.Extensions.FileSystemGlobbing"/> explicitly.
/// </summary>
public static class GlobMatcherExtensions
{
    /// <inheritdoc cref="Matcher.Execute"/>
    public static PatternMatchingResult Execute(this Matcher matcher, DirectoryInfo directory)
    {
        return matcher.Execute(new DirectoryInfoWrapper(directory));
    }
}