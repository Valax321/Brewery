using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace Brewery.ProjectTool.Commands.Build;

[Verb("build", HelpText = "Builds the specified GBA project."),
 SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class BuildCommandOptions : IToolCommandOptions
{
    [Value(0, Required = true, HelpText = "The path to the .brewproj file.")]
    public string ProjectPath { get; set; } = string.Empty;

    [Option('c', "configuration", HelpText = "The configuration name to build with.")]
    public string BuildConfiguration { get; set; } = string.Empty;
}