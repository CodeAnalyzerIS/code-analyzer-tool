using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem.Loaders;

public class BuiltinPluginLoader : IPluginLoader
{
    private readonly GlobalConfig _globalConfig;
    private readonly IEnumerable<IPlugin> _builtInPlugins;

    public BuiltinPluginLoader(GlobalConfig globalConfig, IEnumerable<IPlugin> builtInPlugins)
    {
        _globalConfig = globalConfig;
        _builtInPlugins = builtInPlugins;
    }

    public Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return _builtInPlugins
            .SelectMany(GetPluginWithConfig)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private Dictionary<PluginConfig, IPlugin> GetPluginWithConfig(IPlugin plugin)
    {
        var config = GetBuiltInPluginConfigs().SingleOrDefault(c => c.PluginName == plugin.PluginName);
        if (config is null) return new Dictionary<PluginConfig, IPlugin>();
        return new Dictionary<PluginConfig, IPlugin>
        {
            [config] = plugin
        };
    }
    
    private IEnumerable<PluginConfig> GetBuiltInPluginConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: null });
    }
}