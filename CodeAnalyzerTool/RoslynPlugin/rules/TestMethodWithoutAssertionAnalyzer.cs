using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin_API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestMethodWithoutAssertionAnalyzer : Rule
{
    // ReSharper disable once MemberCanBePrivate.Global
    // public const string DiagnosticId = "NoAssertion";
    public sealed override string DiagnosticId => "NoAssertion";
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    private const string Category = "Syntax";
    private readonly DiagnosticDescriptor _rule;

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
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Warning;
        _rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

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

        var diagnostic = Diagnostic.Create(_rule, 
            methodDeclaration.GetFirstToken().GetLocation(),
            effectiveSeverity: Severity,
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