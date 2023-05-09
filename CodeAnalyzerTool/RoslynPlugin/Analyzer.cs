using System.Collections.Immutable;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

// todo REFACTOR Console.WriteLines TO LOGS (and removeReSharper ignore)
// ReSharper disable LocalizableElement

namespace RoslynPlugin;

public static class Analyzer {
    internal static async Task<ImmutableArray<Diagnostic>> StartAnalysis(MSBuildWorkspace workspace,
        PluginConfig pluginConfig, string pluginsPath) {
        var workingDirectory = Directory.GetCurrentDirectory();
        var solutionPaths = Directory.GetFiles(workingDirectory, StringResources.SolutionSearchPattern,
            SearchOption.AllDirectories);
        var diagnosticResults = new List<Diagnostic>();

        foreach (var solutionPath in solutionPaths) {
            Console.WriteLine($"Loading Solution '{solutionPath}'");

            var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
            Console.WriteLine($"Finished loading solution '{solutionPath}'");

            var analyzers = RuleLoader.LoadRules(workingDirectory, pluginConfig, pluginsPath);

            var projects = solution.Projects;

            foreach (var project in projects) {
                var diagnostics = await AnalyseProject(project, analyzers);
                if (!diagnostics.IsEmpty) diagnosticResults.AddRange(diagnostics);
            }
        }

        return diagnosticResults.ToImmutableArray();
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyseProject(Project project,
        ImmutableArray<DiagnosticAnalyzer> analyzers) {
        Console.WriteLine($"Analyzing project: {project.Name}\n=========================================");
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new NullReferenceException(StringResources.NullCompilationMsg);

        if (analyzers.IsEmpty) return new List<Diagnostic>().ToImmutableArray();

        var diagnosticResults = compilation.WithAnalyzers(analyzers)
            .GetAnalyzerDiagnosticsAsync().Result;

        Console.WriteLine($@"Diagnostics found: {diagnosticResults.Length}");
        foreach (var diagnostic in diagnosticResults) {
            Console.WriteLine(diagnostic.ToString());
        }

        return diagnosticResults;
    }
}