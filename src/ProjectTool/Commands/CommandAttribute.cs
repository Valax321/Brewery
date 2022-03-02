using JetBrains.Annotations;

namespace Brewery.ProjectTool.Commands;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal class CommandAttribute : Attribute
{
    public Type OptionsType { get; }

    public CommandAttribute(Type optionsType)
    {
        OptionsType = optionsType;
    }
}