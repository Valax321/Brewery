#if !SHIPPING
using Brewery.ToolSdk.Logging;

namespace Brewery.ProjectTool.Commands.ErrorTest;

[Command(typeof(ErrorTestCommandOptions))]
internal class ErrorTestCommand : IToolCommand
{
    private readonly ILogger<ErrorTestCommand> m_logger;

    public ErrorTestCommand(ILogger<ErrorTestCommand> logger)
    {
        m_logger = logger;
    }

    public void Run()
    {
        m_logger.Info("Throwing error...");
        throw new Exception("Exception triggered by testerror command");
    }
}
#endif