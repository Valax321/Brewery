namespace Brewery.ToolSdk.Registry;

/// <summary>
/// Interface for type registries.
/// </summary>
/// <typeparam name="TClass">The type of object this registry will accept.</typeparam>
public interface IRegistry<TClass>
{
    /// <summary>
    /// All names registered in this <see cref="IRegistry{TClass}"/>
    /// </summary>
    IEnumerable<string> Names { get; }

    /// <summary>
    /// Registers a new type to this registry.
    /// </summary>
    /// <typeparam name="TClassInstance">The class of item to register.</typeparam>
    /// <param name="name">The name to register the class under.</param>
    /// <returns>Self to allow chaining.</returns>
    public IRegistry<TClass> Register<TClassInstance>(string name) 
        where TClassInstance : TClass, new();

    /// <summary>
    /// Gets an instance of the registered class.
    /// </summary>
    /// <param name="name">The name of the class to search for.</param>
    /// <returns>Instance of the class if found, otherwise null.</returns>
    TClass? GetNamedClass(string name);
}