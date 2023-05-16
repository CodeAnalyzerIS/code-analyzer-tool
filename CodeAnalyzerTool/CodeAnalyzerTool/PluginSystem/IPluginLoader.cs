using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem;

internal interface IPluginLoader
{
    Dictionary<PluginConfig, IPlugin> LoadPlugins();
}