using CAT_API.ConfigModel;

namespace CAT_API;

public interface IPlugin
{
    Task<IEnumerable<RuleViolation>> Analyze(PluginConfig pluginConfig, string pluginsPath);
}