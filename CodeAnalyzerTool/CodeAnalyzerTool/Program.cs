
using RoslynPlugin;

namespace CodeAnalyzerTool;

public class Program {
    static async Task Main()
    {
        await RoslynMain.Analyze();
    }
}