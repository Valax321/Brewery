using System.Reflection;

namespace Brewery.ProjectTool.Utility;

/// <summary>
/// Utility functions for <see cref="Type"/> usage.
/// </summary>
public static class TypeUtility
{
    /// <summary>
    /// Gets all types in the given assembly with the specified attribute.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute to search for.</typeparam>
    /// <param name="assembly">The assembly to search.</param>
    /// <returns>Enumerable of the matching types.</returns>
    public static IEnumerable<Type> GetAllTypesInAssemblyWithAttribute<TAttribute>(this Assembly assembly)
        where TAttribute : Attribute
    {
        return assembly.GetTypes().Where(t => t.GetCustomAttribute<TAttribute>() != null);
    }

    /// <inheritdoc cref="GetAllTypesInAssemblyWithAttribute{TAttribute}(System.Reflection.Assembly)"/>
    public static IEnumerable<Type> GetAllTypesInAssemblyWithAttribute<TAttribute>(this Assembly assembly, Func<Type, TAttribute, bool> predicate)
        where TAttribute : Attribute
    {
        return assembly.GetTypes().Where(t =>
        {
            var attr = t.GetCustomAttribute<TAttribute>();
            return attr != null && predicate(t, attr);
        });
    }
}