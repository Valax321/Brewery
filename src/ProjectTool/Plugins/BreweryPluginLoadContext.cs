using System.Reflection;
using System.Runtime.Loader;

namespace Brewery.ProjectTool.Plugins;

internal class BreweryPluginLoadContext : AssemblyLoadContext
{
    private readonly AssemblyDependencyResolver m_dependencyResolver;

    public BreweryPluginLoadContext(string pluginPath)
    {
        m_dependencyResolver = new AssemblyDependencyResolver(pluginPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        var path = m_dependencyResolver.ResolveAssemblyToPath(assemblyName);
        if (path != null)
        {
            return LoadFromAssemblyPath(path);
        }

        return null;
    }

    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        var libraryPath = m_dependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}