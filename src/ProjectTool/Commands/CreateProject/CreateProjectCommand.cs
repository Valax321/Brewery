#if ENABLE_EXPERIMENTAL_FEATURES
using Brewery.ProjectTool.Utility;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Registry;
using Brewery.ToolSdk.Template;

namespace Brewery.ProjectTool.Commands.CreateProject;

[Command(typeof(CreateProjectCommandOptions))]
internal class CreateProjectCommand : IToolCommand
{
    private readonly ILogger<CreateProjectCommand> m_logger;
    private readonly CreateProjectCommandOptions m_options;
    private readonly IRegistry<ProjectTemplate> m_templates;

    public CreateProjectCommand(ILogger<CreateProjectCommand> logger, 
        CreateProjectCommandOptions options, 
        IRegistry<ProjectTemplate> templates)
    {
        m_logger = logger;
        m_options = options;
        m_templates = templates;
    }

    public void Run()
    {
        if (string.IsNullOrEmpty(m_options.TemplateName))
        {
            ListTemplateTypes();
            return;
        }

        var fullPath = ProjectHelper.ResolveProjectPath(
            m_options.ProjectDirectory, Directory.GetCurrentDirectory());

        var directoryName = Path.GetDirectoryName(fullPath);
        if (string.IsNullOrEmpty(directoryName))
        {
            m_logger.Error($"Could not create project at {fullPath}");
            return;
        }

        var projectFile = Path.Combine(fullPath, directoryName + GameProject.Extension);

        m_logger.Info($"Creating {m_options.TemplateName} project {projectFile}");
    }

    private void ListTemplateTypes()
    {
        m_logger.Info("Project Template Options:");
        foreach (var templateName in m_templates.Names)
        {
            var template = m_templates.GetNamedClass(templateName)!;
            m_logger.Info($"{templateName}: {template.Description}");
        }
    }
}
#endif