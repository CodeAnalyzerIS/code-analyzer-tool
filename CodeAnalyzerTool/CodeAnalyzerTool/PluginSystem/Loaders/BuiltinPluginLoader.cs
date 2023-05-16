using System.Collections;
using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem.Loaders;

public class BuiltinPluginLoader : IPluginLoader
{
    private readonly IEnumerable<PluginConfig> _configs;
    private readonly IEnumerable<IPlugin> _builtInPlugins;

    public BuiltinPluginLoader(GlobalConfig globalConfig, IEnumerable<IPlugin> builtInPlugins)
    {
        _configs = GetBuiltInConfigs(globalConfig);
        _builtInPlugins = builtInPlugins;
    }
    
    private IEnumerable<PluginConfig> GetBuiltInConfigs(GlobalConfig globalConfig)
    {
        return globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: null });
    }

    public Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return _builtInPlugins
            .SelectMany(CreateConfigPluginDictionary)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private Dictionary<PluginConfig, IPlugin> CreateConfigPluginDictionary(IPlugin plugin)
    {
        var config = _configs.SingleOrDefault(c => c.PluginName == plugin.PluginName);
        if (config is null) return new Dictionary<PluginConfig, IPlugin>();
        return new Dictionary<PluginConfig, IPlugin>
        {
            [config] = plugin
        };
    }
}