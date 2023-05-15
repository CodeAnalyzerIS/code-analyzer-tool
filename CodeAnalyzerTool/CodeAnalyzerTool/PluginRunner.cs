using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool;

public class PluginRunner
{

    public async Task<IEnumerable<RuleViolation>> RunPlugins(Dictionary<string, IPlugin> pluginsDictionary,
        IEnumerable<PluginConfig> pluginConfigs, string pluginsPath)
    {
        var configs = pluginConfigs;
        var analysisResults = new List<RuleViolation>();
        foreach (var kv in pluginsDictionary)
        {
            var pluginConfig = configs.Single(p => p.PluginName == kv.Key);
            var pluginResults = await kv.Value.Analyze(pluginConfig, pluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }
}