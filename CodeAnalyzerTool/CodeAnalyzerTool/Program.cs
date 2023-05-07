using CAT_API;
using CAT_API.ConfigModel;

namespace CodeAnalyzerTool;

public class Program
{
    public static async Task Main()
    {
        try
        {
            await SchemaGenerator.GenerateSchema();
            Console.WriteLine(@"Read jsonConfig");
            var globalConfig = await ConfigReader.ReadAsync();

            var analysisResults = await RunPlugins(globalConfig);
            if (analysisResults.Count > 0) analysisResults.ForEach(Console.WriteLine);
            else Console.WriteLine("No problems found! (no analysis results)");

            // todo pass result to backend API (C.A.S.)
        }
        catch (Exception ex)
        {
            // todo fix exception handling
            Console.WriteLine(ex);
        }
    }

    private static async Task<List<AnalysisResult>> RunPlugins(GlobalConfig globalConfig)
    {
        var pluginsDictionary = PluginLoader.LoadPlugins(globalConfig);
        var analysisResults = new List<AnalysisResult>();
        foreach (var kv in pluginsDictionary)
        {
            var pluginConfig = globalConfig.ExternalPlugins.Single(p => p.PluginName == kv.Key);
            var pluginResults = await kv.Value.Analyze(pluginConfig, globalConfig.PluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }
}