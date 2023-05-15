using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool;

public class PluginLoaderComposite : PluginLoaderBase
{
    public ICollection<PluginLoaderBase> PluginLoaders { get; }

    public PluginLoaderComposite()
    {
        PluginLoaders = new List<PluginLoaderBase>();
    }

    public void AddPluginLoader(PluginLoaderBase pluginLoaderBase)
    {
        PluginLoaders.Add(pluginLoaderBase);
    }

    public void RemovePluginLoader(PluginLoaderBase pluginLoaderBase)
    {
        PluginLoaders.Remove(pluginLoaderBase);
    }

    public override Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return PluginLoaders.SelectMany(pl => pl.LoadPlugins())
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}