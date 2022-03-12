#if ENABLE_EXPERIMENTAL_FEATURES
using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace Brewery.ProjectTool.Commands.VSCodeGenerator;

[Verb("generate",
     HelpText = "Generate a Visual Studio Code settings file for auto-completion of project source code."),
 SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal class VSCodeGeneratorCommandOptions : IToolCommandOptions
{
    [Value(0, Required = true, HelpText = "The path to the .brewproj file.")]
    public string ProjectPath { get; set; } = string.Empty;
}
#endif