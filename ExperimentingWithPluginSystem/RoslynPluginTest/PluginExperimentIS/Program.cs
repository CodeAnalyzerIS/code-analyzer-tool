using System.Collections.Immutable;
using System.Reflection;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.MSBuild;

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
     
                    var pluginFolder = Path.Combine(AppContext.BaseDirectory, "lib");
                    var analyzers = LoadPluginAnalyzers(pluginFolder);

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

    private static ImmutableArray<DiagnosticAnalyzer> LoadPluginAnalyzers(string path)
    {
        var assembliesPaths = Directory.GetFiles(path, "*.dll");
        var result = new List<DiagnosticAnalyzer>();
        foreach (var assemblyPath in assembliesPaths)
        {
            var assembly = Assembly.LoadFrom(assemblyPath);
            var analyzers = assembly.GetExportedTypes()
                .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
                .Select(type => Activator.CreateInstance(type) as DiagnosticAnalyzer).ToList();

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

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