using System.Reflection;
using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool;

public abstract class PluginLoaderBase
{
    protected abstract IEnumerable<PluginConfig> GetPluginConfigs();
    public abstract Dictionary<PluginConfig, IPlugin> LoadPlugins();
    
    private protected Assembly LoadPlugin(string assemblyPath)
    {
        var loadContext = new PluginLoadContext(assemblyPath);
        var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath));
        return loadContext.LoadFromAssemblyName(assemblyName);
    }
}