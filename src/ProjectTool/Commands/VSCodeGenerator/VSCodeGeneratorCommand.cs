using Brewery.ToolSdk.Logging;

namespace Brewery.ProjectTool.Commands.VSCodeGenerator;

[Command(typeof(VSCodeGeneratorCommandOptions))]
internal class VSCodeGeneratorCommand : IToolCommand
{
    private readonly ILogger<VSCodeGeneratorCommand> m_logger;
    private readonly VSCodeGeneratorCommandOptions m_options;

    public VSCodeGeneratorCommand(ILogger<VSCodeGeneratorCommand> logger, VSCodeGeneratorCommandOptions options)
    {
        m_logger = logger;
        m_options = options;
    }

    public void Run()
    {
        m_logger.Info($"Generating Visual Studio Code project for {m_options.ProjectPath}");
    }
}