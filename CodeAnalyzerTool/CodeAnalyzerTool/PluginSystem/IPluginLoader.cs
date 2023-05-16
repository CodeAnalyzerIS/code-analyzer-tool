using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem;

public interface IPluginLoader
{
    Dictionary<PluginConfig, IPlugin> LoadPlugins();
}