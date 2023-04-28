using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynPlugin;

public static class RoslynMain
{
    //This main method will be called in the analyzerToolProgram
    public static async Task Analyze() {
        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

        await Analyzer.StartAnalysis(workspace);
    }
}