
using Newtonsoft.Json;
using RoslynPlugin;

namespace CodeAnalyzerTool;

public class Program {
    private static async Task Main()
    {
        // todo pass result to backend API (C.A.S.)
        await SchemaGenerator.GenerateSchema();
        Console.WriteLine(@"Read jsonConfig");
        var globalConfig = await ConfigReader.ReadAsync();
        try
        {
            var roslyn = new RoslynMain();
            var result = await roslyn.Analyze(globalConfig.Plugins.First()); // todo fix plugin config parameter dynamically
        }
        catch (JsonException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}