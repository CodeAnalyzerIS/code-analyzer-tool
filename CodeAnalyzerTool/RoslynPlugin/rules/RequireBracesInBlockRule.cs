﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin_API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class RequireBracesInBlockRule : RoslynRule
{
    public sealed override string DiagnosticId => "RequireBracesInBlock";
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    private const string Category = RuleCategories.Style;
    private readonly DiagnosticDescriptor _rule;
    
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.BraceAnalyzerTitle),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.BraceAnalyzerMessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.BraceAnalyzerDescription), Resources.ResourceManager,
            typeof(Resources));

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public RequireBracesInBlockRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Warning;
        _rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
            DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);
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