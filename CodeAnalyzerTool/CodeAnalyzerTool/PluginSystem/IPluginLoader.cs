using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;

namespace CodeAnalyzerTool.PluginSystem;

/// <summary>
/// Interface for loading plugins and their configurations.
/// </summary>
internal interface IPluginLoader
{ 
    /// <summary>
    /// Loads plugins and returns a dictionary of plugin configurations and corresponding plugin instances.
    /// </summary>
    /// <returns>A dictionary containing key value pairs of a <see cref="PluginConfig" /> and the corresponding plugin.</returns>
    Dictionary<PluginConfig, IPlugin> LoadPlugins();
}