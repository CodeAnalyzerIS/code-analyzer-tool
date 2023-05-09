using System.Reflection;
using System.Text.Json;
using CAT_API;
using CAT_API.ConfigModel;
using RoslynPlugin;

namespace CodeAnalyzerTool;

public static class PluginLoader
{
    public static async Task<List<AnalysisResult>> LoadAndRunPlugins(GlobalConfig globalConfig)
    {
        var analysisResults = new List<AnalysisResult>();
        var externalPluginResults = await RunPlugins(
            pluginsDictionary: LoadExternalPlugins(globalConfig),
            pluginConfigs: globalConfig.ExternalPlugins.ToList(),
            pluginsPath: globalConfig.PluginsPath);
        
        var builtInPluginResults = await RunPlugins(
            pluginsDictionary: LoadBuiltInPlugins(globalConfig),
            pluginConfigs: globalConfig.BuiltInPlugins.ToList(),
            pluginsPath: globalConfig.PluginsPath);
        
        analysisResults.AddRange(externalPluginResults);
        analysisResults.AddRange(builtInPluginResults);
        return analysisResults;
    }

    private static async Task<IEnumerable<AnalysisResult>> RunPlugins(Dictionary<string, IPlugin> pluginsDictionary,
        ICollection<PluginConfig> pluginConfigs, string pluginsPath)
    {
        var analysisResults = new List<AnalysisResult>();
        foreach (var kv in pluginsDictionary)
        {
            var pluginConfig = pluginConfigs.Single(p => p.PluginName == kv.Key);
            var pluginResults = await kv.Value.Analyze(pluginConfig, pluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }

    private static Dictionary<string, IPlugin> LoadBuiltInPlugins(GlobalConfig globalConfig)
    {
        var builtInPlugins = new Dictionary<string, IPlugin>();
        foreach (var pluginConfig in globalConfig.BuiltInPlugins.Where(p => p.Enabled))
        {
            switch (pluginConfig.PluginName)
            {
                case StringResources.RoslynPluginName:
                    builtInPlugins[pluginConfig.PluginName] = new RoslynMain();
                    break;
                default:
                    Console.WriteLine(
                        $"WARNING: {pluginConfig.PluginName} is not a recognized built-in plugin!"); // todo switch to log instead of console print
                    break;
            }
        }

        return builtInPlugins;
    }

    private static Dictionary<string, IPlugin> LoadExternalPlugins(GlobalConfig globalConfig)
    {
        return globalConfig.ExternalPlugins
            .Where(p => p.Enabled)
            .SelectMany(p =>
            {
                if (p.AssemblyName == null)
                    throw new JsonException( // todo move this validation to ConfigReader?
                        "Invalid config file: AssemblyName is a required field for non built-in plugins (external plugins).");
                var pluginPath = Path.Combine(globalConfig.PluginsPath, p.FolderName, p.AssemblyName);
                Assembly pluginAssembly = LoadPlugin(pluginPath);
                return CreatePlugin(pluginAssembly, p.PluginName);
            })
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private static Assembly LoadPlugin(string path)
    {
        Console.WriteLine($"Loading plugin from: {path}");
        var loadContext = new PluginLoadContext(path);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
    }


    private static Dictionary<string, IPlugin> CreatePlugin(Assembly assembly, string pluginName)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type))
            {
                IPlugin? result = Activator.CreateInstance(type) as IPlugin;
                if (result != null)
                {
                    var dic = new Dictionary<string, IPlugin>();
                    dic[pluginName] = result;
                    return dic;
                }
            }
        }

        // todo maybe don't throw exception and stop application when a single plugin can't be loaded
        string availableTypes = string.Join(",", assembly.GetTypes().Select(t => t.FullName));
        throw new ApplicationException(
            $"Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
            $"Available types: {availableTypes}");
    }
}