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

        var type = context.SemanticModel.GetTypeInfo(localDeclaration, context.CancellationToken).Type;
        if (type is null || !CanTypeBeConst(type)) return;

        // if (localDeclaration.Declaration.Type.IsVar) todo check if var can be explicitly declared?


        // foreach (VariableDeclaratorSyntax declarator in localDeclaration.Declaration.Variables)
        // {
        //     ExpressionSyntax value = declarator.Initializer?.Value?.WalkDownParentheses();
        //
        //     if (value?.IsMissing != false)
        //         return;
        //
        //     if (!HasConstantValue(value, typeSymbol, context.SemanticModel, context.CancellationToken))
        //         return;
        // }


        // SyntaxList<StatementSyntax> statements = parent.Kind() switch
        // {
        //     SyntaxKind.Block => ((BlockSyntax)parent).Statements,
        //     SyntaxKind.SwitchSection => ((SwitchSectionSyntax)parent).Statements,
        //     _ => throw new ArgumentOutOfRangeException() // todo
        // };
        //
        // if (statements.Count <= 1)
        //     return;

        return;
    }


    // private static bool HasConstantValue(ExpressionSyntax expression, ITypeSymbol typeSymbol,
    //     SemanticModel semanticModel, CancellationToken cancellationToken = default)
    // {
    //     switch (typeSymbol.SpecialType)
    //     {
    //         case SpecialType.System_Boolean:
    //         {
    //             var kind = expression.Kind();
    //             if (kind is SyntaxKind.TrueLiteralExpression or SyntaxKind.FalseLiteralExpression)
    //                 return true;
    //             
    //             break;
    //         }
    //         case SpecialType.System_Char:
    //         {
    //             if (expression.IsKind(SyntaxKind.CharacterLiteralExpression))
    //                 return true;
    //
    //             break;
    //         }
    //         case SpecialType.System_SByte:
    //         case SpecialType.System_Byte:
    //         case SpecialType.System_Int16:
    //         case SpecialType.System_UInt16:
    //         case SpecialType.System_Int32:
    //         case SpecialType.System_UInt32:
    //         case SpecialType.System_Int64:
    //         case SpecialType.System_UInt64:
    //         case SpecialType.System_Decimal:
    //         case SpecialType.System_Single:
    //         case SpecialType.System_Double:
    //         {
    //             if (expression.IsKind(SyntaxKind.NumericLiteralExpression))
    //                 return true;
    //
    //             break;
    //         }
    //         case SpecialType.System_String:
    //         {
    //             switch (expression.Kind())
    //             {
    //                 case SyntaxKind.StringLiteralExpression:
    //                     return true;
    //                 // case SyntaxKind.InterpolatedStringExpression:
    //                 //     return false; todo check if interpolatedString only contains other constants
    //             }
    //
    //             break;
    //         }
    //     }
    //
    //     return semanticModel.GetConstantValue(expression, cancellationToken).HasValue;
    // }

    private static bool CanTypeBeConst(ITypeSymbol typeSymbol)
    {
        return typeSymbol.IsValueType ||
               typeSymbol.SpecialType == SpecialType.System_String ||
               typeSymbol.TypeKind == TypeKind.Enum;
    }
}