using Brewery.ToolSdk.Registry;

namespace Brewery.ProjectTool.Registry;

/// <summary>
/// Internal IRegistry implementation.
/// </summary>
internal sealed class ClassRegistry<TClass> : IRegistry<TClass>
{
    /// <summary>
    /// If true for this class, then <see cref="GetNamedClass"/> will return a new instance
    /// of the class each time it is called.
    /// </summary>
    public bool InstancesAreTransient { get; set; }

    private readonly Dictionary<string, Func<TClass>> m_classFactories = new();
    private readonly Dictionary<string, TClass> m_instances = new();

    public IEnumerable<string> Names => m_classFactories.Keys;

    public IRegistry<TClass> Register<TClassInstance>(string name) 
        where TClassInstance : TClass, new()
    {
        m_classFactories.TryAdd(name, () =>
        {
            var instance = new TClassInstance();
            if (!InstancesAreTransient)
                m_instances.TryAdd(name, instance);

            return instance;
        });

        return this;
    }

    public TClass? GetNamedClass(string name)
    {
        if (m_instances.TryGetValue(name, out var instance))
            return instance;

        if (m_classFactories.TryGetValue(name, out var factory))
            return factory.Invoke();

        return default;
    }
}