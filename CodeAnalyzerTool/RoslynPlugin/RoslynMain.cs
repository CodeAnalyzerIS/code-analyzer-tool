using CAT_API;
using CAT_API.ConfigModel;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Serilog;

namespace RoslynPlugin;

public class RoslynMain : IPlugin
{
    private static void InitLogger()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }
    
    //This main method will be called in the analyzerToolProgram
    public async Task<IEnumerable<AnalysisResult>> Analyze(PluginConfig pluginConfig, string pluginsPath) {
        InitLogger();
        Log.Information("========================== Roslyn Plugin Start ==========================");
        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (_, e) => Log.Error("{Message}", e.Diagnostic.Message);

        var analyzer = new Analyzer(workspace, pluginConfig, pluginsPath);
        var diagnostics = await analyzer.StartAnalysis();
        Log.Information("========================== Roslyn Plugin End ==========================");
        return DiagnosticConverter.ConvertDiagnostics(diagnostics);
    }
}