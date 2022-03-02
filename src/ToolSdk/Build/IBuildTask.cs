using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Interface for queued build tasks in a <see cref="BuildCommandDispatcher"/>.
/// </summary>
public interface IBuildTask
{
    /// <summary>
    /// Callback for performing logging from this task.
    /// </summary>
    Action<string, LogLevel> Log { set; }

    /// <summary>
    /// Run the build step.
    /// </summary>
    BuildResult Build();
}