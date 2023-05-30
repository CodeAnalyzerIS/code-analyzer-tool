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

        var localDeclarationStatement = (LocalDeclarationStatementSyntax)context.Node;

        if (localDeclarationStatement.IsConst) return; // check whether type is already const

        // check whether the type of the var can legally be declared as const
        var type = context.SemanticModel.GetTypeInfo(localDeclarationStatement.Declaration.Type, context.CancellationToken).Type;
        if (type is null || !CanTypeBeConst(type)) return;
        
        if (!ContainsNoInterpolatedStrings(localDeclarationStatement)) return;
        
        var parent = localDeclarationStatement.Parent;
        if (parent is null) return;
        var statements = GetStatements(parent);
        
        if (!CanLocalVariableBeMadeConst(context, localDeclarationStatement.Declaration.Variables, statements))
            return;

        var diagnostic = Diagnostic.Create(_rule,
            localDeclarationStatement.GetLocation(),
            Severity);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool CanTypeBeConst(ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueType ||
               typeSymbol.SpecialType == SpecialType.System_String ||
               typeSymbol.TypeKind == TypeKind.Enum;
    }

    private static bool ContainsNoInterpolatedStrings(LocalDeclarationStatementSyntax localDeclarationStatement)
    {
        foreach (var variableDeclarator in localDeclarationStatement.Declaration.Variables)
        {
            var initializer = variableDeclarator.Initializer;
            if (initializer is null) return false;
            ExpressionSyntax value = initializer.Value.WalkDownParentheses();
            if (value.IsMissing || value.IsOfSyntaxKind(SyntaxKind.InterpolatedStringExpression)) return false;
        }

        return true;
    }

    private static SyntaxList<StatementSyntax> GetStatements(SyntaxNode parent)
    {
        return parent.Kind() switch
        {
            SyntaxKind.Block => ((BlockSyntax)parent).Statements,
            SyntaxKind.SwitchSection => ((SwitchSectionSyntax)parent).Statements,
            _ => throw new ArgumentOutOfRangeException() // todo
        };
    }

    private static bool CanLocalVariableBeMadeConst(
        SyntaxNodeAnalysisContext context,
        SeparatedSyntaxList<VariableDeclaratorSyntax> variables,
        SyntaxList<StatementSyntax> statements)
    {
        var walker = new MakeLocalVariableConstantWalker(context.SemanticModel, context.CancellationToken);

        foreach (var variable in variables)
        {
            var symbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) as ILocalSymbol;
            if (symbol is not null)
                walker.Identifiers[variable.Identifier.ValueText] = symbol;
        }

        foreach (var statement in statements)
        {
            walker.Visit(statement);
            if (walker.IsVariableNonConstant)
                return false;
        }
        
        return true;
    }
}