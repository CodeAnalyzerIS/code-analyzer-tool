using System.Collections.Immutable;
using CodeAnalyzerTool.API;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using RoslynPlugin.API;

namespace RoslynPlugin.Test;

public class RuleViolationVerifier
{
    private const string DEFAULT_PROJECT_NAME = "ProjectUnderTest.csproj";
    private const string DEFAULT_DOCUMENT_NAME = "SourceUnderTest.cs";
    
    private static readonly MetadataReference[] CommonReferences =
    {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    };
    
    public async Task<IEnumerable<RuleViolation>> CompileStringWithRule(string sourceCode, RoslynRule rule)
    {
        using (Workspace workspace = new AdhocWorkspace())
        {
            Document document = CreateDocument(workspace.CurrentSolution, sourceCode);
            Compilation compilation = await document.Project.GetCompilationAsync() ?? throw new InvalidOperationException();
            var ruleAsAnalyzers = new List<DiagnosticAnalyzer> { rule }.ToImmutableArray();
            var diagnosticResults = await compilation.WithAnalyzers(ruleAsAnalyzers)
                .GetAnalyzerDiagnosticsAsync();
            return DiagnosticConverter.ConvertDiagnostics(diagnosticResults);
        }
    }


    private static Document CreateDocument(Solution solution, string sourceCode)
    {
        const string defaultProjectName = "TestProject";

        ProjectId projectId = ProjectId.CreateNewId();
        ProjectInfo projectInfo = ProjectInfo.Create(
            id: projectId,
            version: VersionStamp.Default,
            name: defaultProjectName,
            assemblyName: defaultProjectName,
            language: "C#");

        // Project project = solution.AddProject(projectInfo).GetProject(projectId) ??
        //                   throw new InvalidOperationException();

        var updatedSolution = solution.AddMetadataReferences(projectId, CommonReferences);

        var project = updatedSolution.GetProject(projectId);

        Document document = project.AddDocument("testFile", SourceText.From(sourceCode));
        
        solution.AddMetadataReferences(projectId, CommonReferences).GetProject(projectId);
        
        return document;
    }
}