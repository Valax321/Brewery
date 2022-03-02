namespace Brewery.ToolSdk.Logging;

/// <summary>
/// Logger levels
/// </summary>
public enum LogLevel
{
    /// <summary>
    /// Messages only printed in debug mode.
    /// </summary>
    Debug,

    /// <summary>
    /// Normal colored informational messages.
    /// </summary>
    Information,

    /// <summary>
    /// Yellow warning messages.
    /// </summary>
    Warning,

    /// <summary>
    /// Red error messages.
    /// </summary>
    Error,

    /// <summary>
    /// Fatal errors.
    /// </summary>
    Fatal
}