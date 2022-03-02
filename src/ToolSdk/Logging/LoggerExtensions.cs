using System;
using System.Collections.Generic;
using System.Text;

namespace Brewery.ToolSdk.Logging;

internal static class LoggerExtensions
{
    public static void Log<T>(this ILogger<T> logger, string message, LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Debug:
                logger.Debug(message);
                break;
            case LogLevel.Information:
                logger.Info(message);
                break;
            case LogLevel.Warning:
                logger.Warn(message);
                break;
            case LogLevel.Error:
                logger.Error(message);
                break;
            case LogLevel.Fatal:
                logger.Fatal(message);
                break;
        }
    }
}