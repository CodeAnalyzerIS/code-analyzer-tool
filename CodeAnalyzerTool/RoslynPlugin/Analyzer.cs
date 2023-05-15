using System.Collections.Immutable;
using CAT_API.ConfigModel;
using CAT_API.Exceptions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Serilog;
using AnalysisResult = CAT_API.AnalysisResult;

namespace RoslynPlugin;

public class Analyzer
{
    private readonly MSBuildWorkspace _workspace;
    private readonly PluginConfig _pluginConfig;
    private readonly string _workingDirectory;
    private readonly ImmutableArray<DiagnosticAnalyzer> _rules;

    public Analyzer(MSBuildWorkspace workspace, PluginConfig pluginConfig, string pluginsPath)
    {
        _workspace = workspace;
        _pluginConfig = pluginConfig;
        _workingDirectory = Directory.GetCurrentDirectory();
        var ruleLoader = new RuleLoader(_workingDirectory, _pluginConfig, pluginsPath);
        _rules = ruleLoader.LoadRules().Cast<DiagnosticAnalyzer>().ToImmutableArray();
    }

    internal async Task<IEnumerable<AnalysisResult>> StartAnalysis()
    {
        var solutionPaths = Directory.GetFiles(_workingDirectory, StringResources.SolutionSearchPattern,
            SearchOption.AllDirectories);
        var analysisResults = new List<AnalysisResult>();
        

        analysisResults.AddRange(await AnalyzeSolutions(solutionPaths));

        return analysisResults;
    }

    private async Task<IEnumerable<AnalysisResult>> AnalyzeSolutions(string[] solutionPaths)
    {
        var analysisResults = new List<AnalysisResult>();
        foreach (var solutionPath in solutionPaths)
        {
            Log.Information("Loading solution: {SolutionPath}", solutionPath);

            var solution = await _workspace.OpenSolutionAsync(solutionPath, new ProgressReporter());

            analysisResults.AddRange(await AnalyzeSolutionProjects(solution.Projects));
        }

        return analysisResults;
    }

    private async Task<IEnumerable<AnalysisResult>> AnalyzeSolutionProjects(IEnumerable<Project> projects)
    {
        var analysisResults = new List<AnalysisResult>();
        foreach (var project in projects)
        {
            try
            {
                analysisResults.AddRange(await AnalyseProject(project));
            }
            catch (Exception ex)
            {
                Log.Error("[{PluginName}]Analyzing project {ProjectName} FAILED, error message: {ErrorMessage}",
                    _pluginConfig.PluginName, project.Name, ex.Message);
            }
        }

        return analysisResults;
    }

    private async Task<IEnumerable<AnalysisResult>> AnalyseProject(Project project)
    {
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new CompilationNotSupportedException(StringResources.NullCompilationMsg);

        var diagnosticResults = await compilation.WithAnalyzers(_rules)
            .GetAnalyzerDiagnosticsAsync();
        var results = DiagnosticConverter.ConvertDiagnostics(diagnosticResults);

        Log.Information("{DiagnosticCount} rule violations detected in project:  {ProjectName}",
            $"{diagnosticResults.Length,-4}", project.Name);
        return results;
    }
}