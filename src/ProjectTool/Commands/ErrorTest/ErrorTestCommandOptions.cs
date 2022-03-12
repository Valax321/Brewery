#if !SHIPPING
using CommandLine;

namespace Brewery.ProjectTool.Commands.ErrorTest;

[Verb("errortest", HelpText = "A testing command that causes an unhandled exception to be thrown.")]
internal class ErrorTestCommandOptions : IToolCommandOptions
{
}
#endif