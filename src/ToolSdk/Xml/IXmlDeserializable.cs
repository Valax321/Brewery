using System.Xml.Linq;

namespace Brewery.ToolSdk.Xml;

/// <summary>
/// Marks an object as being able to be deserialized from a project file.
/// </summary>
public interface IXmlDeserializable
{
    /// <summary>
    /// Method to perform the file reading.
    /// </summary>
    /// <param name="element">The <see cref="XElement"/> to read from.</param>
    void Deserialize(XElement element);
}