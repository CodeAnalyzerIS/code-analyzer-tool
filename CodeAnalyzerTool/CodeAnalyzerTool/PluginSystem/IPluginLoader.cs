using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem;

internal interface IPluginLoader
{
    Dictionary<PluginConfig, IPlugin> LoadPlugins();
}