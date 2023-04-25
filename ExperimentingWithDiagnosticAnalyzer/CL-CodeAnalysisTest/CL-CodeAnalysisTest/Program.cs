using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CL_CodeAnalysisTest
{
    internal class Program
    {
        static async Task Main()
        {
            
            MSBuildLocator.RegisterDefaults();

            using (var workspace = MSBuildWorkspace.Create())
            {
                // Print message for WorkspaceFailed event to help diagnosing project load failures.
                workspace.WorkspaceFailed += (o, e) => Console.WriteLine(e.Diagnostic.Message);
                
                var workingDirectory = Environment.CurrentDirectory;
                Console.WriteLine("******************************\ncsharp_analyzer_IS\n******************************");

                var solutions = Directory.GetFiles(workingDirectory, "*.sln", SearchOption.AllDirectories);
                foreach (var solutionPath in solutions)
                {
                    Console.WriteLine($@"Loading solution '{solutionPath}'");
                    var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
                    Console.WriteLine($@"Finished loading solution '{solutionPath}'");

                    var solutionProjects = solution.Projects;

                    foreach (var project in solutionProjects)
                    {
                        Console.WriteLine($@"Analyzing project: {project}");
                        var compilation = await project.GetCompilationAsync();
                        DiagnosticAnalyzer braceAnalyzer = new BraceAnalyzer();
                        var diagnosticResults = compilation.WithAnalyzers(ImmutableArray.Create(braceAnalyzer, new TestMethodWithoutAssertionAnalyzer()))
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

        private sealed class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                Console.WriteLine($@"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\:ss\.fffffff} {projectDisplay}");
            }
        }
    }
}
