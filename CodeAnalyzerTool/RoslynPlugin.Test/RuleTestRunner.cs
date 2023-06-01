using System.Collections.Immutable;
using CodeAnalyzerTool.API;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using RoslynPlugin.API;

namespace RoslynPlugin.Test;

public static class RuleTestRunner
{
    private const string DEFAULT_PROJECT_NAME = "ProjectUnderTest.csproj";
    private const string DEFAULT_DOCUMENT_NAME = "SourceUnderTest.cs";
    
    private static readonly MetadataReference[] CommonReferences =
    {
        MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
        MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
    };
    
    private static AdhocWorkspace CreateWorkspace(this SourceText source)
    {
        var projectId = ProjectId.CreateNewId();
        var documentId = DocumentId.CreateNewId(projectId, DEFAULT_DOCUMENT_NAME);

        var sourceTextLoader = TextLoader.From(TextAndVersion.Create(source, VersionStamp.Create()));
        var document = DocumentInfo.Create(documentId, DEFAULT_DOCUMENT_NAME)
            .WithTextLoader(sourceTextLoader);
        var project = ProjectInfo.Create(id: projectId,
                version: VersionStamp.Create(),
                name: DEFAULT_PROJECT_NAME,
                assemblyName: DEFAULT_PROJECT_NAME,
                language: LanguageNames.CSharp)
            .WithCompilationOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        var workspace = new AdhocWorkspace();
        var updatedSolution = workspace
            .CurrentSolution
            .AddProject(project)
            .AddMetadataReferences(projectId, CommonReferences)
            .AddDocument(document);

        workspace.TryApplyChanges(updatedSolution);

        return workspace;
    }
    
    private static Project GetProjectUnderTest(this AdhocWorkspace workspace)
    {
        return workspace.CurrentSolution.Projects.First(x => x.Name == DEFAULT_PROJECT_NAME);
    }
    
    private static Document GetDocumentUnderTest(this Project project)
    {
        return project.Documents.First(x => x.Name == DEFAULT_DOCUMENT_NAME);
    }
    
    private static async Task<IEnumerable<RuleViolation>> CompileStringWithRule(string sourceCode, RoslynRule rule)
    {
        var src = SourceText.From(sourceCode);
        var workspace = src.CreateWorkspace();
        var document = workspace.GetProjectUnderTest().GetDocumentUnderTest();
        Compilation compilation = await document.Project.GetCompilationAsync() ?? throw new InvalidOperationException();
        var ruleAsAnalyzers = new List<DiagnosticAnalyzer> { rule }.ToImmutableArray();
        var diagnosticResults = await compilation.WithAnalyzers(ruleAsAnalyzers)
            .GetAnalyzerDiagnosticsAsync();
        return DiagnosticConverter.ConvertDiagnostics(diagnosticResults);
    }
    
    public static async Task ShouldReport(string code, RoslynRule rule)
    {
        var results = await CompileStringWithRule(code, rule);
        results.Should().Contain(rv => rv.Rule.RuleName == rule.RuleName);
    }

    public static async Task ShouldNotReport(string code, RoslynRule rule)
    {
        var results = await CompileStringWithRule(code, rule);
        results.Should().BeEmpty();
    }
}