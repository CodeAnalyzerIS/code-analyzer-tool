using System.Reflection;
using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginLoader;

public abstract class PluginLoaderBase
{
    public abstract Dictionary<PluginConfig, IPlugin> LoadPlugins();
    
    private protected Assembly LoadPlugin(string assemblyPath)
    {
        var loadContext = new PluginLoadContext(assemblyPath);
        var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath));
        return loadContext.LoadFromAssemblyName(assemblyName);
    }
}