using System.Collections.Immutable;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using RoslynPlugin.API;
using Serilog;
using AnalysisResult = CAT_API.AnalysisResult;

namespace RoslynPlugin;

public class Analyzer
{
    private readonly MSBuildWorkspace _workspace;
    private readonly PluginConfig _pluginConfig;
    private readonly string _pluginsPath;

    public Analyzer(MSBuildWorkspace workspace, PluginConfig pluginConfig, string pluginsPath)
    {
        _workspace = workspace;
        _pluginConfig = pluginConfig;
        _pluginsPath = pluginsPath;
    }

    internal async Task<IEnumerable<AnalysisResult>> StartAnalysis()
    {
        var workingDirectory = Directory.GetCurrentDirectory();
        var solutionPaths = Directory.GetFiles(workingDirectory, StringResources.SolutionSearchPattern,
            SearchOption.AllDirectories);
        var diagnosticResults = new List<AnalysisResult>();
        var ruleLoader = new RuleLoader(workingDirectory, _pluginConfig, _pluginsPath);
        var rules = ruleLoader.LoadRules();

        foreach (var solutionPath in solutionPaths)
        {
            Log.Information("Loading solution: {SolutionPath}", solutionPath);

            var solution = await _workspace.OpenSolutionAsync(solutionPath, new ProgressReporter());
            var projects = solution.Projects;

            foreach (var project in projects)
            {
                try
                {
                    var diagnostics = await AnalyseProject(project, rules);
                    if (diagnostics.Any()) diagnosticResults.AddRange(diagnostics);
                }
                catch (Exception ex)
                {
                    Log.Error("[{PluginName}]Analyzing project {ProjectName} FAILED, error message: {ErrorMessage}",
                        _pluginConfig.PluginName, project.Name, ex.Message);
                }
            }
        }

        return diagnosticResults;
    }

    private async Task<IEnumerable<AnalysisResult>> AnalyseProject(Project project, IEnumerable<RoslynRule> rules)
    {
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new NullReferenceException(StringResources.NullCompilationMsg);

        var rulesAsDiagnosticAnalyzer = rules.Cast<DiagnosticAnalyzer>().ToImmutableArray();

        var diagnosticResults = await compilation.WithAnalyzers(rulesAsDiagnosticAnalyzer)
            .GetAnalyzerDiagnosticsAsync();
        var results = DiagnosticConverter.ConvertDiagnostics(diagnosticResults);

        Log.Information("{DiagnosticCount} rule violations detected in project:  {ProjectName}",
            $"{diagnosticResults.Length,-4}", project.Name);
        return results;
    }
}