using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;
using Serilog;

namespace CodeAnalyzerTool;

public class BuiltinPluginLoader : PluginLoaderBase
{
    private readonly GlobalConfig _globalConfig;

    public BuiltinPluginLoader(GlobalConfig globalConfig)
    {
        _globalConfig = globalConfig;
    }
    
    protected override IEnumerable<PluginConfig> GetPluginConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: null });
    }

    public override Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        var builtInPlugins = new Dictionary<PluginConfig, IPlugin>();
        foreach (var pluginConfig in GetPluginConfigs())
        {
            Log.Information("Loading built-in plugin: {PluginName}", pluginConfig.PluginName);
            switch (pluginConfig.PluginName)
            {
                case StringResources.ROSLYN_PLUGIN_NAME:
                    builtInPlugins[pluginConfig] = new RoslynPlugin.RoslynPlugin();
                    break;
                default:
                    Log.Error("Loading built-in plugin failed: {PluginName} is not a recognized built-in plugin",
                        pluginConfig.PluginName);
                    break;
            }
        }

        return builtInPlugins;
    }
}