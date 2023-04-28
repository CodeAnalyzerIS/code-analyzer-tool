using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynPlugin; 

public static class Analyzer {
    internal static async Task<ImmutableArray<Diagnostic>> StartAnalysis(MSBuildWorkspace workspace) {
        var workingDirectory = Directory.GetCurrentDirectory();
        var solutionPaths = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.AllDirectories);
        ImmutableArray<Diagnostic> diagnosticResults = new ImmutableArray<Diagnostic>();
        
        foreach (var solutionPath in solutionPaths) {
            Console.WriteLine($@"Loading Solution '{solutionPath}'");

            var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
            Console.WriteLine($@"Finished loading solution '{solutionPath}'");

            var projects = solution.Projects;
            
            foreach (var project in projects) {
                var diagnostics = await AnalyseProject(project, workingDirectory);
                diagnosticResults = diagnosticResults.AddRange(diagnostics);
            }
        }
        return diagnosticResults;
    }

    private static async Task<ImmutableArray<Diagnostic>> AnalyseProject(Project project, string workingDir) {
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new NullReferenceException("Compilation was null");
        
        var analyzers = RuleLoader.LoadRules(workingDir);

        var diagnosticResults = compilation.WithAnalyzers(analyzers)
            .GetAnalyzerDiagnosticsAsync().Result;

        Console.WriteLine($@"Diagnostics found: {diagnosticResults.Length}");
        foreach (var diagnostic in diagnosticResults) {
            Console.WriteLine(diagnostic.ToString());
        }

        return diagnosticResults;
    }
}