using System.Drawing;
using Brewery.ToolSdk.Logging;
using Console = Colorful.Console;

namespace Brewery.ProjectTool.Logging;

internal class LogConsoleProvider : ILogProvider
{
    public void Log(string message, LogLevel level)
    {
        switch (level)
        {
            case LogLevel.Debug:
                Console.WriteLine(message, Color.LightGray);
                break;
            case LogLevel.Information:
                Console.WriteLine(message, Color.White);
                break;
            case LogLevel.Warning:
                Console.WriteLine(message, Color.Yellow);
                break;
            case LogLevel.Error:
                Console.WriteLine(message, Color.Red);
                break;
            case LogLevel.Fatal:
                Console.WriteLine($"FATAL: {message}", Color.Red);
                break;
            default:
                throw new Exception("Unimplemented log level");
        }
    }
}