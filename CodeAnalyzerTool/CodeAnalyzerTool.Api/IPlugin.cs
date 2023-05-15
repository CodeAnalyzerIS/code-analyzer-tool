using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.Api;

public interface IPlugin
{
    Task<IEnumerable<RuleViolation>> Analyze(PluginConfig pluginConfig, string pluginsPath);
}