using System.Xml.Linq;
using Brewery.ToolSdk.Xml;
using Microsoft.Extensions.FileSystemGlobbing;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// Base class for all build rules.
/// </summary>
public abstract class BuildRule : IXmlDeserializable
{
    /// <summary>
    /// Glob pattern matcher for files targeted by this rule.
    /// </summary>
    public Matcher Target { get; } = new();

    /// <summary>
    /// Key-value collection of all properties on this rule.
    /// Can be used to access properties that aren't explicitly
    /// read into members.
    /// </summary>
    public RulePropertyCollection Properties { get; } = new();

    /// <inheritdoc />
    public virtual void Deserialize(XElement element)
    {
        element.ReadAttribute<string>(nameof(Target), value => Target.AddInclude(value));

        foreach (var prop in element.Attributes())
        {
            Properties.Add(prop.Name.LocalName, prop.Value);
        }
    }
}