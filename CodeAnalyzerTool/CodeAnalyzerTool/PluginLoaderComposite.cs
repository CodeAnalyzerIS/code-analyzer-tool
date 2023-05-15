using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool;

public class PluginLoaderComposite : PluginLoaderBase
{
    private readonly GlobalConfig _globalConfig;
    public ICollection<PluginLoaderBase> PluginLoaders { get; }

    public PluginLoaderComposite(GlobalConfig globalConfig)
    {
        _globalConfig = globalConfig;
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


    protected override IEnumerable<PluginConfig> GetPluginConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: not null });
    }

    public override Dictionary<string, IPlugin> LoadPlugins()
    {
        return PluginLoaders.SelectMany(pl => pl.LoadPlugins())
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}