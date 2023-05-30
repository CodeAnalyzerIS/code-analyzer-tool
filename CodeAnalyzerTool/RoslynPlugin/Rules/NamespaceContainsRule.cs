using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;
using Serilog;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class NamespaceContainsRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.NAMESPACE_CONTAINS_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string CATEGORY = RuleCategories.NAMING;
    private readonly DiagnosticDescriptor _rule;
    public const string NAMESPACE_OPTION_KEY = "namespace";

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.NamespaceContains_Title),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.NamespaceContains_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.NamespaceContains_Description), Resources.ResourceManager,
            typeof(Resources));

    public NamespaceContainsRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Info;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY,
            Severity, isEnabledByDefault: true, description: Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        if (!Options.TryGetValue(NAMESPACE_OPTION_KEY, out _))
        {
            Log.Warning(StringResources.NO_NAME_SPACE_OPTION_MSG);
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
            Options[NAMESPACE_OPTION_KEY]);
        ctx.ReportDiagnostic(diagnostic);
    }

    private bool ContainsGivenNameSpace(NamespaceDeclarationSyntax namespaceDeclaration)
    {
        var namespaceDeclarationName = namespaceDeclaration.Name.ToString();
        return namespaceDeclarationName.Contains(Options[NAMESPACE_OPTION_KEY]);
    }
}