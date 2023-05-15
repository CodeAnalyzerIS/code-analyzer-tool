using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginLoader;

public class BuiltinPluginLoader : IPluginLoader
{
    private readonly GlobalConfig _globalConfig;
    private readonly IEnumerable<IPlugin> _builtInPlugins;

    public BuiltinPluginLoader(GlobalConfig globalConfig, IEnumerable<IPlugin> builtInPlugins)
    {
        _globalConfig = globalConfig;
        _builtInPlugins = builtInPlugins;
    }

    private Dictionary<PluginConfig, IPlugin> GetPluginWithConfig(IPlugin plugin)
    {
        var config = GetBuiltInConfigs().SingleOrDefault(c => c.PluginName == plugin.PluginName);
        if (config is null) return new Dictionary<PluginConfig, IPlugin>();
        return new Dictionary<PluginConfig, IPlugin>
        {
            [config] = plugin
        };
    }

    public Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return _builtInPlugins
            .SelectMany(GetPluginWithConfig)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    
    private IEnumerable<PluginConfig> GetBuiltInConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: null });
    }
}