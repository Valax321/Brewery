using System.ComponentModel;
using System.Text;
using System.Xml.Linq;
using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Xml;

/// <summary>
/// Extensions for reading properties from <see cref="XDocument"/> nodes.
/// </summary>
public static class XExtensions
{
    /// <summary>
    /// Reads an attribute on this element and injects its value via the writeCallback parameter.
    /// </summary>
    /// <typeparam name="T">The type to read.</typeparam>
    /// <param name="element">The element to read attributes from.</param>
    /// <param name="attributeName">The name of the attribute to read.</param>
    /// <param name="writeCallback">Method that uses the read attribute value.</param>
    /// <param name="errorOnMissing">Should a <see cref="GameProjectReadException"/> be thrown if the attribute is not found?</param>
    /// <returns>The input <see cref="XElement"/> to allow chaining.</returns>
    public static XElement ReadAttribute<T>(this XElement element, string attributeName, Action<T> writeCallback, bool errorOnMissing = false)
    {
        var attribute = element.Attribute(attributeName);
        if (attribute is not null)
        {
            T value;

            if (typeof(T).IsEnum)
            {
                value = (T)Enum.Parse(typeof(T), attribute.Value);
            }
            else if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                value = (T)Convert.ChangeType(attribute.Value, typeof(T));
            }
            else 
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                value = (T)converter.ConvertFromString(attribute.Value)!;
            }

            writeCallback(value);
        }
        else if (errorOnMissing)
            throw new GameProjectReadException(
                $"Could not find attribute {attribute} on property {element.GetElementPath()}");

        return element;
    }

    /// <summary>
    /// Reads a child element of this element and injects its value via the writeCallback parameter.
    /// </summary>
    /// <typeparam name="T">The type to read.</typeparam>
    /// <param name="element">The element to read value from.</param>
    /// <param name="propertyName">The name of the element to read.</param>
    /// <param name="writeCallback">Method that uses the read element value.</param>
    /// <param name="errorOnMissing">Should a <see cref="GameProjectReadException"/> be thrown if the property is not found?</param>
    /// <returns>The input <see cref="XElement"/> to allow chaining.</returns>
    public static XElement ReadProperty<T>(this XElement element, string propertyName, Action<T> writeCallback, bool errorOnMissing = false)
    {
        var property = element.Element(propertyName);
        if (property is not null)
        {
            T value;

            if (typeof(IXmlDeserializable).IsAssignableFrom(typeof(T)))
            {
                value = Activator.CreateInstance<T>()!;
                ((IXmlDeserializable)value).Deserialize(property);
            }
            else if (typeof(T).IsEnum)
            {
                value = (T)Enum.Parse(typeof(T), property.Value);
            }
            else if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                value = (T)Convert.ChangeType(property.Value, typeof(T));
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                value = (T)converter.ConvertFromString(property.Value)!;
            }

            writeCallback(value);
        }
        else if (errorOnMissing)
            throw new GameProjectReadException(
                $"Could not find property named {element.GetElementPath()}");

        return element;
    }

    /// <summary>
    /// Reads a child element of this element as a list of the given type
    /// and injects its value via the writeCallback parameter.
    /// </summary>
    /// <typeparam name="T">The type of element to read.</typeparam>
    /// <param name="element">The element to read values from.</param>
    /// <param name="propertyName">The name of the element to read.</param>
    /// <param name="listElementName">The name of each list element.</param>
    /// <param name="writeCallback">Method that uses the read element value.</param>
    /// <param name="errorOnMissing">Should a <see cref="GameProjectReadException"/> be thrown if the property is not found?</param>
    /// <returns>The input <see cref="XElement"/> to allow chaining.</returns>
    public static XElement ReadListProperty<T>(this XElement element, string propertyName, string listElementName,
        Action<List<T>> writeCallback, bool errorOnMissing = false)
    {
        var property = element.Element(propertyName);
        if (property is not null)
        {
            var list = new List<T>();

            foreach (var item in property.Elements(listElementName))
            {
                T value;

                if (typeof(IXmlDeserializable).IsAssignableFrom(typeof(T)))
                {
                    value = Activator.CreateInstance<T>()!;
                    ((IXmlDeserializable)value).Deserialize(property);
                }
                else if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
                {
                    value = (T)Convert.ChangeType(item.Value, typeof(T));
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    value = (T)converter.ConvertFromString(item.Value);
                }

                list.Add(value);
            }

            writeCallback(list);
        }
        else if (errorOnMissing)
            throw new GameProjectReadException(
                $"Could not find property named {element.GetElementPath()}");

        return element;
    }

    private static string GetElementPath(this XElement element)
    {
        var sb = new StringBuilder();
        var elem = element;
        do
        {
            sb.Insert(0, "." + elem.Name);
        } 
        while ((elem = element.Parent) != null);

        // Remove the root .
        sb.Remove(0, 1);

        return sb.ToString();
    }
}