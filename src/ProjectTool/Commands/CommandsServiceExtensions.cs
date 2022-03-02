using System.Reflection;
using Brewery.ProjectTool.Utility;
using CommandLine;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.ProjectTool.Commands;

internal static class CommandsServiceExtensions
{
    public static IServiceCollection AddCommands(this IServiceCollection services, IEnumerable<string> args, Action? parseErrorsCallback = null)
    {
        Parser.Default.ParseArguments(args, Assembly.GetEntryAssembly()!
                .GetAllTypesInAssemblyWithAttribute<VerbAttribute>().ToArray())
            .WithParsed(o => CreateCommand(o as IToolCommandOptions ?? throw new InvalidOperationException(),
                services))
            .WithNotParsed(_ => parseErrorsCallback?.Invoke());

        return services;
    }

    private static void CreateCommand(IToolCommandOptions options, IServiceCollection services)
    {
        services.AddSingleton(options.GetType(), options);
        services.AddSingleton(typeof(IToolCommand),
            Assembly.GetEntryAssembly()!.GetAllTypesInAssemblyWithAttribute<CommandAttribute>(
                (type, attribute) => type.IsAssignableTo(typeof(IToolCommand)) && attribute.OptionsType == options.GetType()).First());
    }
}