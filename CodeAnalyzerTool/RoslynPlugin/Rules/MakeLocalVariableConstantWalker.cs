using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

internal class MakeLocalVariableConstantWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    private readonly KeyValuePair<string, ILocalSymbol> _variableIdentifier;
    private readonly CancellationToken _cancellationToken;
    public bool IsVariableNonConstant { get; set; }

    public MakeLocalVariableConstantWalker(SemanticModel semanticModel, KeyValuePair<string, ILocalSymbol> variableIdentifier,
        CancellationToken cancellationToken, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) : base(depth)
    {
        _semanticModel = semanticModel;
        _variableIdentifier = variableIdentifier;
        _cancellationToken = cancellationToken;
        IsVariableNonConstant = false;
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (FromExtensionMethod(node) && IsIdentifierAMatch(node))
        {
            var methodSymbol = _semanticModel.GetSymbolInfo(node.Parent!, _cancellationToken).Symbol as IMethodSymbol;
            var firstParameter = methodSymbol?.ReducedFrom?.Parameters.FirstOrDefault();
            if (firstParameter is not null && IsOutOrRef(firstParameter)) IsVariableNonConstant = true;
        }

        base.VisitIdentifierName(node);
    }

    private static bool FromExtensionMethod(IdentifierNameSyntax node)
    {
        return node.Parent is not null && node.IsParentOfSyntaxKind(SyntaxKind.SimpleMemberAccessExpression);
    }

    private static bool IsOutOrRef(IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.RefKind is RefKind.Out or RefKind.Ref;
    }
    
     public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        CheckIdentifierMatches(node.Left);
        Visit(node.Left);
        Visit(node.Right);
    }

     private void CheckIdentifierMatches(ExpressionSyntax expression)
     {
         if (IsIdentifierAMatch(expression)) IsVariableNonConstant = true;
     }
     
     private bool IsIdentifierAMatch(SyntaxNode node)
     {
         return node is IdentifierNameSyntax identifierName && IsIdentifierAMatch(identifierName);
     }
     
     private bool IsIdentifierAMatch(IdentifierNameSyntax identifierName)
     {
         var identifierStringMatches  = identifierName.Identifier.ValueText == _variableIdentifier.Key;
         var symbol = _semanticModel.GetSymbolInfo(identifierName, _cancellationToken).Symbol;
         var identifierSymbolMatches = SymbolEqualityComparer.Default.Equals(_variableIdentifier.Value, symbol);

         return identifierStringMatches && identifierSymbolMatches;
     }

    public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
    {
        if (node.IsOfSyntaxKind(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression))
        {
            CheckIdentifierMatches(node.Operand);
            Visit(node.Operand);
        }
        else
        {
            base.VisitPrefixUnaryExpression(node);
        }
    }
    
    public override void VisitPostfixUnaryExpression(PostfixUnaryExpressionSyntax node)
    {
        if (node.IsOfSyntaxKind(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression))
        {
            CheckIdentifierMatches(node.Operand);
            Visit(node.Operand);
        }
        else
        {
            base.VisitPostfixUnaryExpression(node);
        }
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        if (node.RefKindKeyword.IsOfSyntaxKind(SyntaxKind.RefKeyword, SyntaxKind.OutKeyword))
        {
            CheckIdentifierMatches(node.Expression);
        }
        base.VisitArgument(node);
    }
}