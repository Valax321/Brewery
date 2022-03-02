using Brewery.ToolSdk.Logging;

namespace Brewery.ProjectTool.Logging;

internal class ToolLogger<T> : ILogger<T>
{
    private readonly ILogProvider m_provider;

    public ToolLogger(ILogProvider provider)
    {
        m_provider = provider;
    }

    public void Debug(string message)
    {
        if (IsDebug)
            m_provider.Log($"[{typeof(T)}] {message}", LogLevel.Debug);
    }

    public void Info(string message)
    {
        m_provider.Log(message, LogLevel.Information);
    }

    public void Warn(string message)
    {
        m_provider.Log(message, LogLevel.Warning);
    }

    public void Error(string message)
    {
        m_provider.Log(message, LogLevel.Error);
    }

    public void Fatal(string message)
    {
        m_provider.Log(message, LogLevel.Fatal);
    }

    private static bool IsDebug
    {
        get
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}