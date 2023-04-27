using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;
using RoslynPluginTest.rules;

namespace RoslynPluginTest;

public class Program
{
    static async Task Main()
    {
        MSBuildLocator.RegisterDefaults();

        using (var workspace = MSBuildWorkspace.Create())
        {
            // Print message for WorkspaceFailed event to help diagnosing project load failures.
            workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);
            
            var workingDirectory = Directory.GetCurrentDirectory();
            Console.WriteLine(workingDirectory);
            var solutionPaths = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.AllDirectories);
            foreach (var solutionPath in solutionPaths)
            {
                Console.WriteLine($@"Loading solution '{solutionPath}'");

                // Attach progress reporter so we print projects as they are loaded.
                var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
                Console.WriteLine($@"Finished loading solution '{solutionPath}'");

                var projects = solution.Projects;

                foreach (var project in projects)
                {
                    var compilation = await project.GetCompilationAsync();
                    if (compilation == null) throw new NullReferenceException("Compilation was null");
     
                    // var pluginFolder = Path.Combine(AppContext.BaseDirectory, "lib");
                    var analyzers = LoadRules(workingDirectory);

                    var diagnosticResults = compilation.WithAnalyzers(analyzers)
                        .GetAnalyzerDiagnosticsAsync().Result;

                    Console.WriteLine($@"Diagnostics found: {diagnosticResults.Length}");
                    if (!diagnosticResults.IsEmpty)
                        foreach (var diagnostic in diagnosticResults)
                        {
                            Console.WriteLine(diagnostic.ToString());
                        }
                }
            }
        }
    }

    private static ImmutableArray<DiagnosticAnalyzer> LoadRules(string workingDir) {
        var roslynRules = Path.Combine(workingDir, "CAT/Roslyn/");
        Console.WriteLine(roslynRules);
        var externalRules = Directory.GetFiles(roslynRules, "*.dll");
        var result = new List<DiagnosticAnalyzer>();
        foreach (var rulePath in externalRules)
        {
            var assembly = Assembly.LoadFrom(rulePath);
            var analyzers = assembly.GetExportedTypes()
                .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type => Activator.CreateInstance(type) as DiagnosticAnalyzer).ToList();

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }
        result.Add(new TestMethodWithoutAssertionAnalyzer());

        return result.ToImmutableArray();
    }

    private sealed class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
    {
        public void Report(ProjectLoadProgress loadProgress)
        {
            var projectDisplay = Path.GetFileName(loadProgress.FilePath);
            if (loadProgress.TargetFramework != null)
            {
                projectDisplay += $" ({loadProgress.TargetFramework})";
            }

            Console.WriteLine(
                $@"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\:ss\.fffffff} {projectDisplay}");
        }
    }
}