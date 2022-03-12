#if ENABLE_EXPERIMENTAL_FEATURES
using CommandLine;

namespace Brewery.ProjectTool.Commands.CreateProject;

[Verb("new", HelpText = "Create a new project.")]
internal class CreateProjectCommandOptions : IToolCommandOptions
{
    [Value(0, HelpText = "The name of the template to create the project from.")]
    public string TemplateName { get; set; } = string.Empty;

    [Option('p', "path", Default = "", HelpText = "Directory where the project should be created.")]
    public string ProjectDirectory { get; set; } = string.Empty;
}
#endif