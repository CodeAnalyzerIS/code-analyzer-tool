using CodeAnalyzerTool.API.ConfigModel;

namespace CodeAnalyzerTool.API;

public interface IPlugin
{
    string PluginName { get; }
    Task<IEnumerable<RuleViolation>> Analyze(PluginConfig pluginConfig, string? pluginsPath);
}