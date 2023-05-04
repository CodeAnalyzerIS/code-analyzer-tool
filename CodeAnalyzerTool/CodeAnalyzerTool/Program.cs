
using System.Reflection;
using CAT_API;
using Newtonsoft.Json;

namespace CodeAnalyzerTool;

public class Program {
    private static async Task Main()
    {
        try
        {
            await SchemaGenerator.GenerateSchema();
            Console.WriteLine(@"Read jsonConfig");
            var globalConfig = await ConfigReader.ReadAsync();
            var directoryPath = globalConfig.PluginsPath;
            var pluginNames = globalConfig.Plugins.Select(p => p.PluginName);
            var pluginsDictionary = pluginNames.SelectMany(name =>
            {
                var path = Path.Combine(globalConfig.PluginsPath, name);
                Assembly pluginAssembly = LoadPlugin(path);
                return CreatePlugin(pluginAssembly, name);
            })
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            
            // Dictionary<string, IPlugin> plugins = pluginNames.SelectMany(name =>
            // {
            //     var path = Path.Combine(globalConfig.PluginsPath, name);
            //     Assembly pluginAssembly = LoadPlugin(path);
            //     return CreatePlugin(pluginAssembly, name);
            // }).ToList();

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
    
    
    static Assembly LoadPlugin(string relativePath)
    {
        // // Navigate up to the solution root
        // string root = Path.GetFullPath(Path.Combine(
        //     Path.GetDirectoryName(
        //         Path.GetDirectoryName(
        //             Path.GetDirectoryName(
        //                 Path.GetDirectoryName(
        //                     Path.GetDirectoryName(typeof(Program).Assembly.Location)))))));

        // string pluginLocation = Path.GetFullPath(Path.Combine(root, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
        // var wd = Directory.GetCurrentDirectory();
        var wd = @"C:\Users\Michel\Documents_Local\repos\Blazor-CRUD-webapp";
        var pluginLocation = Path.Combine(wd, relativePath);
        pluginLocation = Path.Combine(pluginLocation, "RoslynPlugin.dll");
        Console.WriteLine($"Loading plugins from: {pluginLocation}");
        PluginLoadContext loadContext = new PluginLoadContext(pluginLocation);
        return loadContext.LoadFromAssemblyName(new AssemblyName(Path.GetFileNameWithoutExtension(pluginLocation)));
    }
    
    
    static Dictionary<string, IPlugin> CreatePlugin(Assembly assembly, string pluginName)
    {
        
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type))
            {
                IPlugin? result = Activator.CreateInstance(type) as IPlugin;
                if (result != null){
                    var dic =  new Dictionary<string, IPlugin>();
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