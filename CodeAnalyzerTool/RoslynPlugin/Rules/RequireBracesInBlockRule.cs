using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RequireBracesInBlockRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.REQUIRE_BRACES_IN_BLOCK_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public sealed override string? CodeExample => null;
    public sealed override string? CodeExampleFix => null;
    private const string CATEGORY = RuleCategories.STYLE;
    private readonly DiagnosticDescriptor _rule;
    
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.RequireBracesInBlock_Title),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.RequireBracesInBlock_MessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.RequireBracesInBlock_Description), Resources.ResourceManager,
            typeof(Resources));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public RequireBracesInBlockRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Warning;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY,
            Severity, isEnabledByDefault: true, description: Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeBlockStatements);
    }

    private void AnalyzeBlockStatements(SyntaxTreeAnalysisContext ctx)
    {
        var root = ctx.Tree.GetRoot(ctx.CancellationToken);
        foreach (var statement in root.DescendantNodes().OfType<StatementSyntax>())
        {
            if (statement is BlockSyntax) continue;

            if (statement.Parent is not StatementSyntax or BlockSyntax) continue;
            var diagnostic = Diagnostic.Create(_rule, 
                statement.GetFirstToken().GetLocation(),
                effectiveSeverity: Severity,
                null,
                null);
            ctx.ReportDiagnostic(diagnostic);
        }
    }
}