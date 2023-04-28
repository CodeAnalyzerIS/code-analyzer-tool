
using RoslynPlugin;

namespace CodeAnalyzerTool;

public class Program {
    static async Task Main()
    {
        Console.WriteLine(@"Read jsonConfig");
        dynamic jsonObject = await ConfigReader.ReadAsync();
        Console.WriteLine(@"After reading");
        Console.WriteLine(jsonObject?.ToString());
        // Console.WriteLine(jsonObject?.api_url);
        // Console.WriteLine(jsonObject?.pluginsPath);
        await RoslynMain.Analyze();
    }
}