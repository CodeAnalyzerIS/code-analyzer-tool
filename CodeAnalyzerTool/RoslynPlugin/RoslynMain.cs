using CAT_API;
using CAT_API.ConfigModel;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynPlugin;

public class RoslynMain : IPlugin
{
    //This main method will be called in the analyzerToolProgram
    public async Task<IEnumerable<AnalysisResult>> Analyze(PluginConfig pluginConfig) {
        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);

        var diagnostics = await Analyzer.StartAnalysis(workspace, pluginConfig);
        return DiagnosticConverter.ConvertDiagnostics(diagnostics);
    }
}