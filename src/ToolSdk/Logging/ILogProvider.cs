namespace Brewery.ToolSdk.Logging;

/// <summary>
/// Interface for log output.
/// </summary>
public interface ILogProvider
{
    /// <summary>
    /// Logs a message at the given level.
    /// </summary>
    /// <param name="message">The message to log.</param>
    /// <param name="level">The level to log at.</param>
    void Log(string message, LogLevel level);
}