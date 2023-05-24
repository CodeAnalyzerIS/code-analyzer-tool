using System.Collections.Immutable;
using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using RoslynPlugin.Exceptions;
using Serilog;

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
        var ruleLoader = new RuleLoader(_workingDirectory, _pluginConfig, pluginsPath); // todo DI
        _rules = ruleLoader.LoadRules().Cast<DiagnosticAnalyzer>().ToImmutableArray();
    }

    internal async Task<IEnumerable<RuleViolation>> StartAnalysis()
    {
        var solutionPaths = Directory.GetFiles(_workingDirectory, StringResources.SOLUTION_SEARCH_PATTERN,
            SearchOption.AllDirectories);

        var ruleViolations = new List<RuleViolation>();
        foreach (var solutionPath in solutionPaths)
        {
            try
            {
                ruleViolations.AddRange(await AnalyzeSolution(solutionPath));
            }
            catch (Exception ex)
            {
                Log.Error("[{PluginName}]Analyzing solution: {SolutionPath} FAILED, error message: {ErrorMessage}",
                    _pluginConfig.PluginName, solutionPath, ex.Message);
            }
        }
        return ruleViolations;
    }

    private async Task<IEnumerable<RuleViolation>> AnalyzeSolution(string solutionPath)
    {
        Log.Information("Loading solution: {SolutionPath}", solutionPath);
        var solution = await _workspace.OpenSolutionAsync(solutionPath, new ProgressReporter());
        return await AnalyzeSolutionProjects(solution.Projects);
    }

    private async Task<IEnumerable<RuleViolation>> AnalyzeSolutionProjects(IEnumerable<Project> projects)
    {
        var ruleViolations = new List<RuleViolation>();
        foreach (var project in projects)
        {
            try
            {
                ruleViolations.AddRange(await AnalyseProject(project));
            }
            catch (Exception ex)
            {
                Log.Error("[{PluginName}]Analyzing project {ProjectName} FAILED, error message: {ErrorMessage}",
                    _pluginConfig.PluginName, project.Name, ex.Message);
            }
        }

        return ruleViolations;
    }

    private async Task<IEnumerable<RuleViolation>> AnalyseProject(Project project)
    {
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new CompilationNotSupportedException(StringResources.NULL_COMPILATION_MSG);

        var diagnosticResults = await compilation.WithAnalyzers(_rules)
            .GetAnalyzerDiagnosticsAsync();
        var results = DiagnosticConverter.ConvertDiagnostics(diagnosticResults);

        Log.Information("{DiagnosticCount} rule violations detected in project:  {ProjectName}",
            $"{diagnosticResults.Length,-4}", project.Name);
        return results;
    }
}