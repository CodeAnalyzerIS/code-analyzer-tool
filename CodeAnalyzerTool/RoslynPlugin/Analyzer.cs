using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

namespace RoslynPlugin; 

public static class Analyzer {
    public static async Task StartAnalysis(MSBuildWorkspace workspace) {
        var workingDirectory = Directory.GetCurrentDirectory();
        var solutionPaths = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.AllDirectories);
        foreach (var solutionPath in solutionPaths) {
            Console.WriteLine($@"Loading Solution '{solutionPath}'");

            var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
            Console.WriteLine($@"Finished loading solution '{solutionPath}'");

            var projects = solution.Projects;

            foreach (var project in projects) {
                await AnalyseProject(project, workingDirectory);
            }
        }
    }

    private static async Task AnalyseProject(Project project, string workingDir) {
        var compilation = await project.GetCompilationAsync();
        if (compilation == null) throw new NullReferenceException("Compilation was null");
        
        var analyzers = RuleLoader.LoadRules(workingDir);

        var diagnosticResults = compilation.WithAnalyzers(analyzers)
            .GetAnalyzerDiagnosticsAsync().Result;

        Console.WriteLine($@"Diagnostics found: {diagnosticResults.Length}");
        if (!diagnosticResults.IsEmpty) {
            foreach (var diagnostic in diagnosticResults) {
                Console.WriteLine(diagnostic.ToString());
            }
        }
    }
}