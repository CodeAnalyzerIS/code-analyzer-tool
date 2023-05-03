using CAT_API.ConfigModel;

namespace CAT_API;

public interface IPlugin
{
    public Task<IEnumerable<AnalysisResult>> Analyze(PluginConfig pluginConfig, string pluginsPath);
}