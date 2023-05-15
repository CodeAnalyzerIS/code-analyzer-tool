using System.Reflection;
using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;

namespace CodeAnalyzerTool.PluginLoader;

public interface IPluginLoader
{
    Dictionary<PluginConfig, IPlugin> LoadPlugins();
}