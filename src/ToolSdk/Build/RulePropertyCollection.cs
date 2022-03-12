using System.ComponentModel;

namespace Brewery.ToolSdk.Build;

/// <summary>
/// A collection of properties from a source or asset rule.
/// </summary>
public class RulePropertyCollection
{
    private class Property
    {
        public string Value { get; }
        private readonly Dictionary<Type, object> m_typedValues = new();

        public Property(string value)
        {
            Value = value;
        }

        public T Get<T>()
        {
            if (!m_typedValues.ContainsKey(typeof(T)))
                return ParseValue<T>();

            return (T)m_typedValues[typeof(T)];
        }

        private T ParseValue<T>()
        {
            T value;

            if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
            {
                value = (T)Convert.ChangeType(Value, typeof(T));
            }
            else
            {
                var converter = TypeDescriptor.GetConverter(typeof(T));
                value = (T)converter.ConvertFromString(Value);
            }

            m_typedValues.Add(typeof(T), value);
            return value;
        }
    }

    private readonly Dictionary<string, Property> m_properties = new();

    /// <summary>
    /// Gets the given property as the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get the property as.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>The property value, or null if not found.</returns>
    public T? Get<T>(string propertyName)
    {
        if (m_properties.TryGetValue(propertyName, out var prop))
            return prop.Get<T>();

        return default;
    }
    
    /// <summary>
    /// Adds a new property to the collection.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="value">The string value of the property.</param>
    /// <returns>True if the property could be added, otherwise false.</returns>
    public bool Add(string propertyName, string value)
    {
        return m_properties.TryAdd(propertyName, new Property(value));
    }
}