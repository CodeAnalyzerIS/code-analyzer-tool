using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;
using RoslynPlugin.Exceptions;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MakeLocalVariableConstantRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }

    public sealed override string CodeExample => @"class ExampleClass
{
    void ExampleMethod()
    {
        var s = ""This string stays constant"";
    }
}";

    public sealed override string CodeExampleFix => @"class ExampleClass
{
    void ExampleMethod()
    {
        const string s = ""This string stays constant"";
    }
}";
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
        if (localDeclarationStatement.Declaration.Variables.Count > 1) return; // supports only single variable declaration
        var variableDeclarator = localDeclarationStatement.Declaration.Variables.SingleOrDefault();
        if (variableDeclarator is null) return;

        // check whether the type of the var can legally be declared as const
        var type = context.SemanticModel.GetTypeInfo(localDeclarationStatement.Declaration.Type, context.CancellationToken).Type;
        if (type is null || !CanTypeBeConst(type)) return;
        
        if (IsNullOrInterpolatedString(variableDeclarator)) return;
        
        var parent = localDeclarationStatement.Parent;
        if (parent is null) return;
        var statements = GetStatements(parent);

        if (!statements.Any()) return;

        if (!CanBeMadeConst(context, variableDeclarator, statements)) return;

        var props = new Dictionary<string, string?>
        {
            {StringResources.CODE_EXAMPLE_KEY, CodeExample},
            {StringResources.CODE_EXAMPLE_FIX_KEY, CodeExampleFix }
        };

        var props = new Dictionary<string, string?>
        {
            {StringResources.CODE_EXAMPLE_KEY, CodeExample},
            {StringResources.CODE_EXAMPLE_FIX_KEY, CodeExampleFix }
        };

        var diagnostic = Diagnostic.Create(_rule,
            localDeclarationStatement.GetLocation(),
            effectiveSeverity: Severity, null, props.ToImmutableDictionary()
            );
        context.ReportDiagnostic(diagnostic);
    }

    private static bool CanTypeBeConst(ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueType ||
               typeSymbol.SpecialType == SpecialType.System_String ||
               typeSymbol.TypeKind == TypeKind.Enum;
    }

    private static bool IsNullOrInterpolatedString(VariableDeclaratorSyntax variableDeclarator)
    {
        var initializer = variableDeclarator.Initializer;
        if (initializer is null) return true;
        ExpressionSyntax value = initializer.Value.ExtractExpressionFromParentheses();
        if (value.IsMissing || value.IsOfSyntaxKind(SyntaxKind.InterpolatedStringExpression)) return true;
        
        return false;
    }

    private static SyntaxList<StatementSyntax> GetStatements(SyntaxNode parent)
    {
        return parent.Kind() switch
        {
            SyntaxKind.Block => ((BlockSyntax)parent).Statements,
            SyntaxKind.SwitchSection => ((SwitchSectionSyntax)parent).Statements,
            _ => new SyntaxList<StatementSyntax>()
        };
    }

    private static bool CanBeMadeConst(
        SyntaxNodeAnalysisContext context,
        VariableDeclaratorSyntax variable,
        SyntaxList<StatementSyntax> statements)
    {
        var symbol = context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) as ILocalSymbol;
        if (symbol is null) return false;
        
        var identifier = new KeyValuePair<string, ILocalSymbol>(variable.Identifier.ValueText, symbol);
        var walker = new MakeLocalVariableConstantWalker(context.SemanticModel, identifier, context.CancellationToken);
        
        foreach (var statement in statements)
        {
            walker.Visit(statement);
            if (walker.IsVariableNonConstant)
                return false;
        }
        
        return true;
    }
}