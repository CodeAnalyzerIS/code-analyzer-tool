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
        if (node.IsParentOfSyntaxKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.AddressOfExpression)
            && IsLocalReference(node))
        {
            if (node.IsParentOfSyntaxKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var methodSymbol = _semanticModel.GetSymbolInfo(node.Parent, _cancellationToken).Symbol as IMethodSymbol;
                var firstParameter = methodSymbol.ReducedFrom.Parameters.FirstOrDefault();
                if (IsRefOrOut(firstParameter)) IsVariableNonConstant = true;
            }
            else if (node.IsParentOfSyntaxKind(SyntaxKind.AddressOfExpression))
            {
                IsVariableNonConstant = true;
            }
        }

        base.VisitIdentifierName(node);
    }

    private static bool IsRefOrOut(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol is null)
            throw new ArgumentNullException(nameof(parameterSymbol));

        RefKind refKind = parameterSymbol.RefKind;

        return refKind == RefKind.Ref
               || refKind == RefKind.Out;
    }
    
     public override void VisitAssignmentExpression(AssignmentExpressionSyntax node)
    {
        ExpressionSyntax left = node.Left;

        if (node.Kind() == SyntaxKind.SimpleAssignmentExpression
            && (left is TupleExpressionSyntax tupleExpression))
        {
            foreach (ArgumentSyntax argument in tupleExpression.Arguments)
            {
                ExpressionSyntax expression = argument.Expression;

                if (expression?.IsOfSyntaxKind(SyntaxKind.DeclarationExpression) == false)
                    VisitAssignedExpression(expression);

                VisitArgument(argument);
            }
        }
        else
        {
            VisitAssignedExpression(left);
            Visit(left);
        }

        Visit(node.Right);
    }
    
     public void VisitAssignedExpression(ExpressionSyntax expression)
     {
         if (IsLocalReference(expression))
             IsVariableNonConstant = true;
     }
     

     private bool IsLocalReference(SyntaxNode node)
     {
         return node is IdentifierNameSyntax identifierName
                && IsLocalReference(identifierName);
     }
     
     private bool IsLocalReference(IdentifierNameSyntax identifierName)
     {
         return Identifiers.TryGetValue(identifierName.Identifier.ValueText, out ILocalSymbol symbol)
                && SymbolEqualityComparer.Default.Equals(symbol, _semanticModel.GetSymbolInfo(identifierName, _cancellationToken).Symbol);
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