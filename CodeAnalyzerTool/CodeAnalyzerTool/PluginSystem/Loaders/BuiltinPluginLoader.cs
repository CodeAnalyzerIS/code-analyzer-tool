using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem.Loaders;

/// <summary>
/// Class for loading built-in plugins and their configurations.
/// </summary>
internal class BuiltinPluginLoader : IPluginLoader
{
    private readonly IEnumerable<PluginConfig> _configs;
    private readonly IEnumerable<IPlugin> _builtInPlugins;

    /// <summary>
    /// Initializes a new instance of the <see cref="BuiltinPluginLoader"/> class.
    /// </summary>
    /// <param name="globalConfig">The global configuration containing plugin information.</param>
    /// <param name="builtInPlugins">The collection of built-in plugins to load.</param>
    public BuiltinPluginLoader(GlobalConfig globalConfig, IEnumerable<IPlugin> builtInPlugins)
    {
        _configs = GetBuiltInConfigs(globalConfig);
        _builtInPlugins = builtInPlugins;
    }
    
    private IEnumerable<PluginConfig> GetBuiltInConfigs(GlobalConfig globalConfig)
    {
        return globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: null });
    }

    /// <inheritdoc cref="IPluginLoader.LoadPlugins"/>
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