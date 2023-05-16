using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.Api;

public interface IPlugin
{
    string PluginName { get; }
    Task<IEnumerable<RuleViolation>> Analyze(PluginConfig pluginConfig, string pluginsPath);
}