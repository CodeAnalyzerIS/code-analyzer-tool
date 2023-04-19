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

                var solutionPath = @"C:\Users\AlexanderW\source\repos\TestToAnalyse\TestToAnalyse.sln";
                Console.WriteLine($@"Loading solution '{solutionPath}'");

                // Attach progress reporter so we print projects as they are loaded.
                var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
                Console.WriteLine($@"Finished loading solution '{solutionPath}'");

                //Maybe replace with Projects to loop over all projects, since you can now only work with solutions containing one project
                Project currProject = solution.Projects.Single();

                var compilation = await currProject.GetCompilationAsync();
                DiagnosticAnalyzer braceAnalyzer = new BraceAnalyzer();
                var diagnosticResults = compilation.WithAnalyzers(ImmutableArray.Create(braceAnalyzer))
                    .GetAnalyzerDiagnosticsAsync().Result;
                
                Console.WriteLine($@"Diagnostics found: {diagnosticResults.Length}");
                if (!diagnosticResults.IsEmpty)
                    foreach (var diagnostic in diagnosticResults)
                    {
                        Console.WriteLine(diagnostic.ToString());
                    }
                

                // StringBuilder warnings = new StringBuilder();
                //
                // foreach (var document in currProject.Documents)
                // {
                //     var tree = document.GetSyntaxTreeAsync().Result;
                //     var ifStatementNodes = tree?.GetRoot().DescendantNodesAndSelf()
                //         .Where(x => x.IsKind(SyntaxKind.IfStatement));
                //
                //     if (ifStatementNodes != null)
                //         foreach (var node in ifStatementNodes)
                //         {
                //             var ifStatement = node as IfStatementSyntax;
                //
                //             if (ifStatement?.Statement is BlockSyntax blockSyntax && !blockSyntax.Statements.Any())
                //             {
                //                 warnings.Append($"Empty if block is found in file" +
                //                                 $" {document.FilePath} at line" +
                //                                 $" {ifStatement.GetLocation().GetLineSpan().StartLinePosition.Line + 1}" +
                //                                 $" \n");
                //             }
                //         }
                // }
                //
                // if (warnings.Length != 0)
                //     Console.WriteLine(warnings.ToString());
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
