namespace Brewery.ToolSdk.Template;

#if ENABLE_EXPERIMENTAL_FEATURES
/// <summary>
/// Template for a new project.
/// </summary>
public abstract class ProjectTemplate
{
    /// <summary>
    /// Description of the project template.
    /// </summary>
    public abstract string Description { get; }
}
#endif