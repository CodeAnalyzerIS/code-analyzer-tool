using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin_API;
using Serilog;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamespaceRule : RoslynRule
{
    public sealed override string DiagnosticId => "NamespaceContains";
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string Category = RuleCategories.Naming;
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NamespaceCheckTitle),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.NamespaceCheckMessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.NamespaceCheckDescription), Resources.ResourceManager,
            typeof(Resources));

    public NamespaceRule()
    {
        _rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
        Severity = DiagnosticSeverity.Info;
        Options = new Dictionary<string, string>();
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        if (!Options.TryGetValue("namespace", out _))
        {
            Log.Warning(StringResources.NoNameSpaceOptionMsg);
            return;
        }

        context.RegisterSyntaxNodeAction(CheckNamespace, SyntaxKind.NamespaceDeclaration);
    }

    private void CheckNamespace(SyntaxNodeAnalysisContext ctx)
    {
        var nameSpaceDeclaration = (NamespaceDeclarationSyntax)ctx.Node;
        if (ContainsGivenNameSpace(nameSpaceDeclaration)) return;

        var diagnostic = Diagnostic.Create(_rule,
            nameSpaceDeclaration.GetFirstToken().GetLocation(),
            effectiveSeverity: Severity,
            null,
            null,
            Options["namespace"]);
        ctx.ReportDiagnostic(diagnostic);
    }

    private bool ContainsGivenNameSpace(NamespaceDeclarationSyntax namespaceDeclaration)
    {
        var namespaceDeclarationName = namespaceDeclaration.Name.ToString();
        return namespaceDeclarationName.Contains(Options["namespace"]);
    }
}