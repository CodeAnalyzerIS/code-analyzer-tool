using System.Reflection;
using System.Text.Json;
using CAT_API;
using CAT_API.ConfigModel;

namespace CodeAnalyzerTool;

public static class PluginLoader
{
    public static Dictionary<string, IPlugin> LoadPlugins(GlobalConfig globalConfig)
    {
        return globalConfig.ExternalPlugins
            .Where(p => p.Enabled)
            .SelectMany(p =>
            {
                if (p.AssemblyName == null)
                    throw new JsonException(
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