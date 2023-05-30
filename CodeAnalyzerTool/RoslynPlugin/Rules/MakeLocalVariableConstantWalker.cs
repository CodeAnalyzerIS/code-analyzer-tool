using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynPlugin.rules;

internal class MakeLocalVariableConstantWalker : CSharpSyntaxWalker
{
    private readonly SemanticModel _semanticModel;
    public Dictionary<string, ILocalSymbol> Identifiers { get; }
    private readonly CancellationToken _cancellationToken;
    public bool IsVariableNonConstant { get; set; }
    protected bool ShouldVisit => !IsVariableNonConstant;

    public MakeLocalVariableConstantWalker(SemanticModel semanticModel, CancellationToken cancellationToken, SyntaxWalkerDepth depth = SyntaxWalkerDepth.Node) : base(depth)
    {
        _semanticModel = semanticModel;
        _cancellationToken = cancellationToken;
        Identifiers = new();
        IsVariableNonConstant = false;
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (node.Parent is not null && node.IsParentOfSyntaxKind(SyntaxKind.SimpleMemberAccessExpression) && IsLocalReference(node))
        {
            var methodSymbol = _semanticModel.GetSymbolInfo(node.Parent, _cancellationToken).Symbol as IMethodSymbol;
            var firstParameter = methodSymbol?.ReducedFrom?.Parameters.FirstOrDefault();
            if (firstParameter is not null && RefKindIsOutOrRef(firstParameter)) IsVariableNonConstant = true;
        }

        base.VisitIdentifierName(node);
    }

    private static bool RefKindIsOutOrRef(IParameterSymbol parameterSymbol)
    {
        return parameterSymbol.RefKind is RefKind.Out or RefKind.Ref;
    }
    
     public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        VisitAssignedExpression(node.Left);
        Visit(node.Left);
        Visit(node.Right);
    }
    
     public void VisitAssignedExpression(ExpressionSyntax expression)
     {
         if (IsLocalReference(expression))
             IsVariableNonConstant = true;
     }
     
     private bool IsLocalReference(SyntaxNode node)
     {
         return node is IdentifierNameSyntax identifierName && IsLocalReference(identifierName);
     }
     
     private bool IsLocalReference(IdentifierNameSyntax identifierName)
     {
         var isIdentifierRecognized = Identifiers.TryGetValue(identifierName.Identifier.ValueText, out ILocalSymbol? symbol);
         var expectedSymbol = _semanticModel.GetSymbolInfo(identifierName, _cancellationToken).Symbol;
         var areSymbolsEqual = SymbolEqualityComparer.Default.Equals(expectedSymbol, symbol);

         return isIdentifierRecognized && areSymbolsEqual;
     }

    public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
    {
        if (node.IsOfSyntaxKind(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression))
        {
            ExpressionSyntax operand = node.Operand;

            VisitAssignedExpression(operand);
            Visit(operand);
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
            ExpressionSyntax operand = node.Operand;

            VisitAssignedExpression(operand);
            Visit(operand);
        }
        else
        {
            base.VisitPostfixUnaryExpression(node);
        }
    }

    public override void VisitArgument(ArgumentSyntax node)
    {
        switch (node.RefOrOutKeyword.Kind())
        {
            case SyntaxKind.RefKeyword:
                {
                    VisitAssignedExpression(node.Expression);
                    break;
                }
            case SyntaxKind.OutKeyword:
                {
                    ExpressionSyntax expression = node.Expression;

                    if (expression?.IsOfSyntaxKind(SyntaxKind.DeclarationExpression) == false)
                        VisitAssignedExpression(expression);

                    break;
                }
        }

        base.VisitArgument(node);
    }
}