using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using Serilog;

namespace RoslynPlugin;

public class RoslynPlugin : IPlugin
{
    //This main method will be called in the analyzerToolProgram
    public async Task<IEnumerable<RuleViolation>> Analyze(PluginConfig pluginConfig, string pluginsPath) {
        Log.Information("========================== Roslyn Plugin Start ==========================");
        MSBuildLocator.RegisterDefaults();

        using var workspace = MSBuildWorkspace.Create();
        workspace.WorkspaceFailed += (_, e) => Log.Error("{Message}", e.Diagnostic.Message);

        var analyzer = new Analyzer(workspace, pluginConfig, pluginsPath);
        var ruleViolations = await analyzer.StartAnalysis();
        Log.Information("========================== Roslyn Plugin End ==========================");
        return ruleViolations;
    }
}