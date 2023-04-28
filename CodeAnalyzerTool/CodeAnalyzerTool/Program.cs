
using RoslynPlugin;

namespace CodeAnalyzerTool;

public class Program {
    static async Task Main()
    {
        var roslyn = new RoslynMain();
        var result = await roslyn.Analyze();
        Console.WriteLine(result);
    }
}