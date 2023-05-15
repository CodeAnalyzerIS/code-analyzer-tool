using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginLoader;

public class PluginLoaderComposite : IPluginLoader
{
    public ICollection<IPluginLoader> PluginLoaders { get; }

    public PluginLoaderComposite()
    {
        PluginLoaders = new List<IPluginLoader>();
    }

    public void AddPluginLoader(IPluginLoader pluginLoader)
    {
        PluginLoaders.Add(pluginLoader);
    }

    public void RemovePluginLoader(IPluginLoader pluginLoader)
    {
        PluginLoaders.Remove(pluginLoader);
    }

    public Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return PluginLoaders.SelectMany(pl => pl.LoadPlugins())
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}