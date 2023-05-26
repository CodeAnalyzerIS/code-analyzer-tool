using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MakeLocalVariableConstantRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string CATEGORY = RuleCategories.PERFORMANCE;
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantTitle), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantDescription), Resources.ResourceManager, typeof(Resources));

    public MakeLocalVariableConstantRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Info;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY, Severity, true, Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclarationStatement, SyntaxKind.LocalDeclarationStatement);
    }

    private void AnalyzeLocalDeclarationStatement(SyntaxNodeAnalysisContext context)
    {
        if (context.Node.ContainsDiagnostics) return;
        
        var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

        if (localDeclaration.IsConst) return;

        var parent = localDeclaration.Parent;
        if (parent is null) return;
        // var statements = parent.
        
        // todo if else BlockSyntax - SwitchSyntax
        var block = (BlockSyntax)parent;
        var statements = block.Statements;

        return;
    }
}