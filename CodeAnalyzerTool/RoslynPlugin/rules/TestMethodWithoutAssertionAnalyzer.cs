using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestMethodWithoutAssertionAnalyzer : DiagnosticAnalyzer
{
    private const string DiagnosticId = "NoAssertion";
    private const string Category = "Syntax";
    private readonly DiagnosticSeverity _severity;

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.NoAssertionTitle),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.NoAssertionMessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.NoAssertionDescription), Resources.ResourceManager,
            typeof(Resources));

    public TestMethodWithoutAssertionAnalyzer()
    {
        _severity = DiagnosticSeverity.Warning;
    }

    public TestMethodWithoutAssertionAnalyzer(DiagnosticSeverity severity)
    {
        _severity = severity;
    }

    private static readonly DiagnosticDescriptor Rule = new(DiagnosticId, Title, MessageFormat, Category,
        DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
    
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethod(SyntaxNodeAnalysisContext ctx)
    {
        var methodDeclaration = (MethodDeclarationSyntax)ctx.Node;

        if (!IsTestMethod(methodDeclaration)) return;
        if (ContainsAssertion(methodDeclaration)) return;

        // var descriptor = CreateDescriptor();
        var diagnostic = Diagnostic.Create(Rule, 
            methodDeclaration.GetFirstToken().GetLocation(),
            effectiveSeverity: _severity,
            null,
            null,
            methodDeclaration.Identifier.ValueText);
        ctx.ReportDiagnostic(diagnostic);
    }

    private static bool IsTestMethod(MethodDeclarationSyntax methodDeclaration)
    {
        var attributes = methodDeclaration.AttributeLists.SelectMany(al => al.Attributes);
        return attributes.Any(a => a.Name.ToString().StartsWith("Test"));
    }

    private static bool ContainsAssertion(MethodDeclarationSyntax methodDeclaration)
    {
        var assertionExpressions = methodDeclaration.DescendantNodes()
            .OfType<InvocationExpressionSyntax>()
            .Select(ies => ies.Expression.ToString());

        return assertionExpressions.Any(ae => ae.StartsWith("Assert."));
    }
}