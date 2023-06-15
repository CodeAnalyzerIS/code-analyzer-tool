using System.Reflection;
using System.Runtime.Loader;

namespace CodeAnalyzerTool.PluginSystem;

/// <summary>
/// Represents a custom assembly load context used for loading plugins.
/// </summary>
internal class PluginLoadContext : AssemblyLoadContext
{
    private AssemblyDependencyResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="PluginLoadContext"/> class with the specified plugin path.
    /// </summary>
    /// <param name="pluginPath">The path to the plugin assembly.</param>

    public PluginLoadContext(string pluginPath)
    {
        _resolver = new AssemblyDependencyResolver(pluginPath);
    }

    /// <summary>
    /// Loads the specified assembly using the assembly name.
    /// </summary>
    /// <param name="assemblyName">The name of the assembly to load.</param>
    /// <returns>The loaded assembly, or <c>null</c> if it couldn't be loaded.</returns>
    protected override Assembly? Load(AssemblyName assemblyName)
    {
        string? assemblyPath = _resolver.ResolveAssemblyToPath(assemblyName);
        if (assemblyPath != null)
        {
            return LoadFromAssemblyPath(assemblyPath);
        }

        return null;
    }

    /// <summary>
    /// Loads the specified unmanaged DLL using the DLL name.
    /// </summary>
    /// <param name="unmanagedDllName">The name of the unmanaged DLL to load.</param>
    /// <returns>A handle to the loaded unmanaged DLL, or <c>IntPtr.Zero"</c> if it couldn't be loaded.</returns>
    protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
    {
        string? libraryPath = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);
        if (libraryPath != null)
        {
            return LoadUnmanagedDllFromPath(libraryPath);
        }

        return IntPtr.Zero;
    }
}