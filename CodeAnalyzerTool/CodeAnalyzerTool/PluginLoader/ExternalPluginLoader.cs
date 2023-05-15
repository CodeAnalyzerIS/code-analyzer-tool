using System.Reflection;
using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;
using CodeAnalyzerTool.Api.Exceptions;
using Serilog;

namespace CodeAnalyzerTool.PluginLoader;

public class ExternalPluginLoader : PluginLoaderBase
{
    private readonly GlobalConfig _globalConfig;

    public ExternalPluginLoader(GlobalConfig globalConfig)
    {
        _globalConfig = globalConfig;
    }

    public override Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return GetExternalConfigs()
            .SelectMany(LoadExternalPlugin)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private IEnumerable<PluginConfig> GetExternalConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: not null });
    }
    
    private Dictionary<PluginConfig, IPlugin> LoadExternalPlugin(PluginConfig config)
    {
        try
        {
            Log.Information("Loading external plugin: {PluginName}", config.PluginName);
            if (config.AssemblyName == null)
                throw new ConfigException(StringResources.ASSEMBLY_NAME_MISSING_MESSAGE);
            var pluginAssemblyPath = Path.Combine(_globalConfig.PluginsPath, config.FolderName, config.AssemblyName);
            Assembly pluginAssembly = LoadPlugin(pluginAssemblyPath);
            return GetPluginWithConfig(pluginAssembly, config);
        }
        catch (Exception ex)
        {
            Log.Error("Loading external plugin failed: {ErrorMessage}", ex.Message);
            return new Dictionary<PluginConfig, IPlugin>();
        }
    }
    
    private Dictionary<PluginConfig, IPlugin> GetPluginWithConfig(Assembly assembly, PluginConfig config)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;

            IPlugin? result = Activator.CreateInstance(type) as IPlugin;
            if (result != null)
            {
                return new Dictionary<PluginConfig, IPlugin>
                {
                    [config] = result
                };
            }
        }

        throw new TypeLoadException($"Can't find any type which implements IPlugin in {assembly.Location}.");
    }
}