using System.Reflection;
using CAT_API;
using Newtonsoft.Json;

namespace CodeAnalyzerTool;

public class Program
{
    private static async Task Main()
    {
        try
        {
            await SchemaGenerator.GenerateSchema();
            Console.WriteLine(@"Read jsonConfig");
            var globalConfig = await ConfigReader.ReadAsync();
            var directoryPath = globalConfig.PluginsPath;
            var pluginNames = globalConfig.Plugins.Select(p => p.PluginName);
            var pluginsDictionary = pluginNames.SelectMany(pluginName =>
                {
                    var pluginPath = Path.Combine(directoryPath, globalConfig.PluginsPath, pluginName + ".dll"); // todo maybe change that plugin main .dll file name doesn't need to be name of the plugin
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugin(pluginAssembly, pluginName);
                })
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var analysisResults = new List<AnalysisResult>();
            foreach (var kv in pluginsDictionary)
            {
                var pluginConfig = globalConfig.Plugins.Single(p => p.PluginName == kv.Key);
                var pluginResults = await kv.Value.Analyze(pluginConfig, directoryPath);
                analysisResults.AddRange(pluginResults);
            }

            // todo pass result to backend API (C.A.S.)
            Console.WriteLine(analysisResults);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }


    static Assembly LoadPlugin(string path)
    {
        Console.WriteLine($"Loading plugins from: {path}");
        PluginLoadContext loadContext = new PluginLoadContext(path);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(path)));
    }


    static Dictionary<string, IPlugin> CreatePlugin(Assembly assembly, string pluginName)
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