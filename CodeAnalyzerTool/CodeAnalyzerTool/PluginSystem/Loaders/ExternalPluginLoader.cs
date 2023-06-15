using System.Reflection;
using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;
using CodeAnalyzerTool.API.Exceptions;
using Serilog;

namespace CodeAnalyzerTool.PluginSystem.Loaders;

/// <summary>
/// Class for loading external plugins and their configurations.
/// </summary>
internal class ExternalPluginLoader : IPluginLoader
{
    private readonly GlobalConfig _globalConfig;

    /// <summary>
    /// Initializes a new instance of the <c>ExternalPluginLoader</c> class.
    /// </summary>
    /// <param name="globalConfig">The global configuration containing information necessary for loading external plugins.</param>
    public ExternalPluginLoader(GlobalConfig globalConfig)
    {
        _globalConfig = globalConfig;
    }

    /// <inheritdoc cref="IPluginLoader.LoadPlugins"/>
    public Dictionary<PluginConfig, IPlugin> LoadPlugins()
    {
        return GetExternalPluginConfigs()
            .SelectMany(LoadExternalPlugin)
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private IEnumerable<PluginConfig> GetExternalPluginConfigs()
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
            Assembly pluginAssembly = LoadWithContext(pluginAssemblyPath);
            return CreateConfigPluginDictionary(pluginAssembly, config);
        }
        catch (Exception ex)
        {
            Log.Error("Loading external plugin failed: {ErrorMessage}", ex.Message);
            return new Dictionary<PluginConfig, IPlugin>();
        }
    }
    
    private Assembly LoadWithContext(string assemblyPath)
    {
        var loadContext = new PluginLoadContext(assemblyPath);
        var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath));
        return loadContext.LoadFromAssemblyName(assemblyName);
    }
    
    private Dictionary<PluginConfig, IPlugin> CreateConfigPluginDictionary(Assembly assembly, PluginConfig config)
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