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

            var analysisResults = await PluginLoader.LoadAndRunPlugins(globalConfig);
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
}