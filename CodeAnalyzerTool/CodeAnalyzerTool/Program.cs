
using CodeAnalyzerTool.ConfigModel;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using RoslynPlugin;

namespace CodeAnalyzerTool;

public class Program {
    static async Task Main()
    {
        // var generator = new JSchemaGenerator();
        // var schema = generator.Generate(typeof(ToolConfig));
        // Console.WriteLine(schema);
        Console.WriteLine(@"Read jsonConfig");
        dynamic? jsonObject = await ConfigReader.ReadAsync();
        Console.WriteLine(@"After reading");
        Console.WriteLine(jsonObject?.ApiUrl);
        Console.WriteLine(jsonObject?.PluginsPath);
        if (jsonObject?.Plugins.Count > 0)
        {
            foreach (var plugin in jsonObject?.Plugins)
            {
                Console.WriteLine(plugin.PluginName);
                if (plugin.Rules.Count <= 0) continue;
                foreach (var rule in plugin.Rules)
                {
                        Console.WriteLine(rule.RuleName);
                        Console.WriteLine(rule.Enabled);
                        foreach (var option in rule.Options)
                        {
                            Console.WriteLine(option.Key);
                            Console.WriteLine(option.Value);
                        }
                }
            }
        }
        // Console.WriteLine(jsonObject?.api_url);
        // Console.WriteLine(jsonObject?.pluginsPath);
        // await RoslynMain.Analyze();
    }
}