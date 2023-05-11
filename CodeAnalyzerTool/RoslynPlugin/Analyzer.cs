using System.Collections.Immutable;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using Serilog;

namespace RoslynPlugin;

public static class Analyzer
{
    internal static async Task<ImmutableArray<Diagnostic>> StartAnalysis(MSBuildWorkspace workspace,
        PluginConfig pluginConfig, string pluginsPath)
    {
        var workingDirectory = Directory.GetCurrentDirectory();
        var solutionPaths = Directory.GetFiles(workingDirectory, StringResources.SolutionSearchPattern,
            SearchOption.AllDirectories);
        var diagnosticResults = new List<Diagnostic>();

        foreach (var solutionPath in solutionPaths)
        {
            Log.Information("Loading solution: {SolutionPath}", solutionPath);

            var solution = await workspace.OpenSolutionAsync(solutionPath, new ProgressReporter());
            var analyzers = RuleLoader.LoadRules(workingDirectory, pluginConfig, pluginsPath);
            var projects = solution.Projects;

            foreach (var project in projects)
            {
                try
                {
                    var diagnostics = await AnalyseProject(project, analyzers);
                    if (!diagnostics.IsEmpty) diagnosticResults.AddRange(diagnostics);
                }
                catch (Exception ex)
                {
                    Log.Error("[{PluginName}]Analyzing project {ProjectName} FAILED, error message: {ErrorMessage}",
                        pluginConfig.PluginName, project.Name, ex.Message);
                }
            }
        }

        return diagnosticResults.ToImmutableArray();
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyseProject(Project project,
        ImmutableArray<DiagnosticAnalyzer> analyzers)
    {
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new NullReferenceException(StringResources.NullCompilationMsg);

        if (analyzers.IsEmpty) return new List<Diagnostic>().ToImmutableArray();

        var diagnosticResults = await compilation.WithAnalyzers(analyzers)
            .GetAnalyzerDiagnosticsAsync();

        Log.Information("{DiagnosticCount} rule violations detected in project:  {ProjectName}",
            $"{diagnosticResults.Length,-4}", project.Name);
        return diagnosticResults;
    }
}