using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class TestMethodWithoutAssertionRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.TEST_METHOD_WITHOUT_ASSERTION_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public sealed override string CodeExample => @"class ExampleClass
{
    [Test]
    void ExampleTest()
    {
        // ... (No assertions present)
    }
}";

    public sealed override string CodeExampleFix => @"class ExampleClass
{
    [Test]
    void ExampleTest()
    {
        // ...
        Assert.True(exampleExpression); // (any other asserts like Assert.Equals, etc are also valid)
    }
}";
    private const string CATEGORY = RuleCategories.MAINTAINABILITY;
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.TestMethodWithoutAssertion_Title), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.TestMethodWithoutAssertion_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.TestMethodWithoutAssertion_Description), Resources.ResourceManager, typeof(Resources));

    public TestMethodWithoutAssertionRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Warning;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY, Severity, true, Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                               GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeMethod(SyntaxNodeAnalysisContext ctx)
    {
        var methodDeclaration = (MethodDeclarationSyntax)ctx.Node;

        if (!IsTestMethod(methodDeclaration)) return;
        if (ContainsAssertion(methodDeclaration)) return;
        
        var props = new Dictionary<string, string?>
        {
            {StringResources.CODE_EXAMPLE_KEY, CodeExample},
            {StringResources.CODE_EXAMPLE_FIX_KEY, CodeExampleFix }
        };

        var diagnostic = Diagnostic.Create(_rule,
            methodDeclaration.GetFirstToken().GetLocation(),
            effectiveSeverity: Severity,
            null,
            props.ToImmutableDictionary(),
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