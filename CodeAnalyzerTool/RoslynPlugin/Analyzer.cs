using System.Collections.Immutable;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
// todo remove Console.WriteLines (and removeReSharper ignore)
// ReSharper disable LocalizableElement

namespace RoslynPlugin; 

public static class Analyzer {
    internal static async Task<ImmutableArray<Diagnostic>> StartAnalysis(MSBuildWorkspace workspace, PluginConfig pluginConfig) {
        // var workingDirectory = Directory.GetCurrentDirectory();
        var workingDirectory = @"C:\Users\AlexanderW\Documents\Blazor-CRUD-webapp";
        var solutionPaths = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.AllDirectories);
        var diagnosticResults = new List<Diagnostic>();
        
        foreach (var solutionPath in solutionPaths) {
            Console.WriteLine($"Loading Solution '{solutionPath}'");

            var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
            Console.WriteLine($"Finished loading solution '{solutionPath}'");
            
            var analyzers = RuleLoader.LoadRules(workingDirectory);

            var projects = solution.Projects;
            
            foreach (var project in projects) {
                var diagnostics = await AnalyseProject(project, pluginConfig, analyzers);
                if (diagnostics.Length > 0) diagnosticResults.AddRange(diagnostics);
            }
        }

        return diagnosticResults.ToImmutableArray();
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyseProject(Project project, PluginConfig pluginConfig, 
        ImmutableArray<DiagnosticAnalyzer> analyzers) {
        Console.WriteLine($"Analyzing project: {project.Name}\n=========================================");
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new NullReferenceException("Compilation was null");
        
        var diagnosticResults = compilation.WithAnalyzers(analyzers)
            .GetAnalyzerDiagnosticsAsync().Result;

        Console.WriteLine($@"Diagnostics found: {diagnosticResults.Length}");
        foreach (var diagnostic in diagnosticResults) {
            Console.WriteLine(diagnostic.ToString());
        }

        return diagnosticResults;
    }
}