using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool;

public class PluginRunner
{
    public async Task<IEnumerable<RuleViolation>> RunPlugins(Dictionary<PluginConfig, IPlugin> pluginsDictionary, string pluginsPath)
    {
        var analysisResults = new List<RuleViolation>();
        foreach (var (config, plugin) in pluginsDictionary)
        {
            var pluginResults = await plugin.Analyze(config, pluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }
}