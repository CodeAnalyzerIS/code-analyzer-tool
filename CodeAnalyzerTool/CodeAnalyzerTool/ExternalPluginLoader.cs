using System.Reflection;
using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;
using CodeAnalyzerTool.Api.Exceptions;
using Serilog;

namespace CodeAnalyzerTool;

public class ExternalPluginLoader : PluginLoaderBase
{
    private readonly GlobalConfig _globalConfig;

    public ExternalPluginLoader(GlobalConfig globalConfig)
    {
        _globalConfig = globalConfig;
    }
    
    protected override IEnumerable<PluginConfig> GetPluginConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: not null });
    }

    public override Dictionary<string, IPlugin> LoadPlugins()
    {
        return GetPluginConfigs()
            .SelectMany(pluginConfig => LoadExternalPlugin(_globalConfig.PluginsPath, pluginConfig))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }
    
    private Dictionary<string, IPlugin> LoadExternalPlugin(string pluginsPath, PluginConfig config)
    {
        try
        {
            Log.Information("Loading external plugin: {PluginName}", config.PluginName);
            if (config.AssemblyName == null)
                throw new ConfigException(StringResources.ASSEMBLY_NAME_MISSING_MESSAGE);
            var pluginAssemblyPath = Path.Combine(pluginsPath, config.FolderName, config.AssemblyName);
            Assembly pluginAssembly = LoadPlugin(pluginAssemblyPath);
            return CreateExternalPlugin(pluginAssembly, config.PluginName);
        }
        catch (Exception ex)
        {
            Log.Error("Loading external plugin failed: {ErrorMessage}", ex.Message);
            return new Dictionary<string, IPlugin>();
        }
    }
    
    private Dictionary<string, IPlugin> CreateExternalPlugin(Assembly assembly, string pluginName)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;

            IPlugin? result = Activator.CreateInstance(type) as IPlugin;
            if (result != null)
            {
                return new Dictionary<string, IPlugin>
                {
                    [pluginName] = result
                };
            }
        }

        throw new TypeLoadException($"Can't find any type which implements IPlugin in {assembly.Location}.");
    }
}