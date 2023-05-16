using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem;

public class PluginRunner
{
    private readonly IEnumerable<IPluginLoader> _pluginLoaders;
    private readonly GlobalConfig _globalConfig;
    public PluginRunner(IEnumerable<IPluginLoader> pluginLoaders, GlobalConfig globalConfig)
    {
        _pluginLoaders = pluginLoaders;
        _globalConfig = globalConfig;
    }

    public async Task<IEnumerable<RuleViolation>> Run()
    {
        var loadedPluginsWithConfigs = RunPluginLoaders();
        return await RunPlugins(loadedPluginsWithConfigs, _globalConfig.PluginsPath);
    }

    private Dictionary<PluginConfig, IPlugin> RunPluginLoaders()
    {
        return _pluginLoaders.SelectMany(pl => pl.LoadPlugins())
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private async Task<IEnumerable<RuleViolation>> RunPlugins(Dictionary<PluginConfig, IPlugin> loadedPluginsWithConfigs, string pluginsPath)
    {
        var analysisResults = new List<RuleViolation>();
        foreach (var (config, plugin) in loadedPluginsWithConfigs)
        {
            var pluginResults = await plugin.Analyze(config, pluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }
}