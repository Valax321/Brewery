namespace Brewery.ToolSdk.Project;

/// <summary>
/// Result of a <see cref="GameProject.Build"/> call.
/// </summary>
public enum BuildResult
{
    /// <summary>
    /// The build was successful.
    /// </summary>
    Succeeded,
    /// <summary>
    /// The build failed.
    /// </summary>
    Failed
}