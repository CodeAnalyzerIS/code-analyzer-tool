using System.Reflection;
using CAT_API;

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
            // todo move loading plugins to separate class
            var pluginsDirectoryPath = globalConfig.PluginsPath;
            var pluginsDictionary = globalConfig.ExternalPlugins
                .Where(p => p.Enabled)
                .SelectMany(p =>
                {
                    // todo check that AssemblyName is not null
                    var pluginPath = Path.Combine(pluginsDirectoryPath, p.FolderName, p.AssemblyName);
                    Assembly pluginAssembly = LoadPlugin(pluginPath);
                    return CreatePlugin(pluginAssembly, p.PluginName);
                })
                .ToDictionary(pair => pair.Key, pair => pair.Value);

            var analysisResults = new List<AnalysisResult>();
            foreach (var kv in pluginsDictionary)
            {
                var pluginConfig = globalConfig.ExternalPlugins.Single(p => p.PluginName == kv.Key);
                var pluginResults = await kv.Value.Analyze(pluginConfig, pluginsDirectoryPath);
                analysisResults.AddRange(pluginResults);
            }

            // todo pass result to backend API (C.A.S.)
            Console.WriteLine(analysisResults);
            analysisResults.ForEach(result => Console.WriteLine(result));
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