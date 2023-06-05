using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem;

internal class PluginRunner
{
    private readonly IEnumerable<IPluginLoader> _pluginLoaders;
    private readonly GlobalConfig _globalConfig;

    public PluginRunner(IEnumerable<IPluginLoader> pluginLoaders, GlobalConfig globalConfig)
    {
        _pluginLoaders = pluginLoaders;
        _globalConfig = globalConfig;
    }

    public async Task<AnalysisResult> Run()
    {
        var loadedPluginsWithConfigs = RunPluginLoaders();
        var ruleViolations = await RunPlugins(loadedPluginsWithConfigs, _globalConfig.PluginsPath);

        return CheckFailStatus(ruleViolations);
    }

    private Dictionary<PluginConfig, IPlugin> RunPluginLoaders()
    {
        return _pluginLoaders.SelectMany(pl => pl.LoadPlugins())
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private async Task<IEnumerable<RuleViolation>> RunPlugins(
        Dictionary<PluginConfig, IPlugin> loadedPluginsWithConfigs, string? pluginsPath)
    {
        var analysisResults = new List<RuleViolation>();
        foreach (var (config, plugin) in loadedPluginsWithConfigs)
        {
            var pluginResults = await plugin.Analyze(config, pluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }

    private AnalysisResult CheckFailStatus(IEnumerable<RuleViolation> ruleViolations)
    {
        return _globalConfig.FailSeverityThreshold switch
        {
            Severity.Error when ruleViolations.Any(rv => rv.Severity is Severity.Error) => new AnalysisResult(
                StatusCode.Failed, ruleViolations),
            Severity.Warning when ruleViolations.Any(rv => rv.Severity is Severity.Error or Severity.Warning) =>
                new AnalysisResult(StatusCode.Failed, ruleViolations),
            Severity.Info when ruleViolations.Any() => new AnalysisResult(StatusCode.Failed, ruleViolations),
            _ => new AnalysisResult(StatusCode.Success, ruleViolations)
        };
    }
}