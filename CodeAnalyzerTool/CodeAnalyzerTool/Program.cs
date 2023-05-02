
using RoslynPlugin;

namespace CodeAnalyzerTool;

public class Program {
    static async Task Main()
    {
        var roslyn = new RoslynMain();
        var result = await roslyn.Analyze();
        // todo pass result to backend API (C.A.S.)
    }
}