namespace Brewery.ToolSdk.Logging
{
    /// <summary>
    /// Logger interface.
    /// </summary>
    public interface ILogger<T>
    {
        /// <summary>
        /// Log a debug-only message.
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Log a normal informational message.
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Log a warning message.
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);

        /// <summary>
        /// Log a user error message.
        /// </summary>
        /// <param name="message"></param>
        void Error(string message);

        /// <summary>
        /// Log a tool runtime error.
        /// </summary>
        /// <param name="message"></param>
        void Fatal(string message);
    }
}
