using Brewery.ProjectTool.Utility;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.ProjectTool.Commands.Build;

[Command(typeof(BuildCommandOptions))]
internal class BuildCommand : IToolCommand
{
    private readonly ILogger<BuildCommand> m_logger;
    private readonly BuildCommandOptions m_options;
    private readonly IServiceProvider m_services;

    public BuildCommand(ILogger<BuildCommand> logger, BuildCommandOptions options, IServiceProvider services)
    {
        m_logger = logger;
        m_options = options;
        m_services = services;
    }

    public void Run()
    {
        var fullProjectPath = ProjectHelper.ResolveProjectPath(
            m_options.ProjectPath, Directory.GetCurrentDirectory());

        if (Path.GetExtension(fullProjectPath) != GameProject.Extension)
        {
            m_logger.Error($"Project file must have {GameProject.Extension} extension.");
            return;
        }

        FileInfo projectFile;

        try
        {
            projectFile = new FileInfo(fullProjectPath);
        }
        catch (Exception ex)
        {
            m_logger.Error($"Failed to get project file info: {ex.Message}");
#if DEBUG
            throw;
#else
            return;
#endif
        }

        if (!projectFile.Exists)
        {
            m_logger.Error($"Failed to find project at {fullProjectPath}.");
            return;
        }

        try
        {
            var project = GameProject.Read(projectFile, m_services, string.IsNullOrEmpty(m_options.BuildConfiguration) 
                ? "Release" : m_options.BuildConfiguration);
            PerformBuild(project);
        }
        catch (GameProjectReadException ex)
        {
            m_logger.Error($"Failed to read project: {ex.InnerException?.Message ?? ex.Message}");
#if DEBUG
            throw;
#else
            return;
#endif
        }
    }

    private void PerformBuild(GameProject project)
    {
        m_logger.Info($"Building project {project.ProjectDirectory}");
        m_logger.Info($"Build configuration: {project.Configuration}");
        m_logger.Debug(project.ToString());

        var result = project.Build();
        m_logger.Info($"Project build result: {result.ToString().ToUpper()}");
    }
}