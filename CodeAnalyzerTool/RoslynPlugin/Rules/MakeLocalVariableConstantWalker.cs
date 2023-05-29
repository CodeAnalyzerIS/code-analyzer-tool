using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynPlugin.rules;

internal class MakeLocalVariableConstantWalker : CSharpSyntaxWalker
{
    [ThreadStatic]
    private static MakeLocalVariableConstantWalker? _cachedInstance;

    public Dictionary<string, ILocalSymbol> Identifiers { get; } = new();

    public SemanticModel SemanticModel { get; set; }

    public CancellationToken CancellationToken { get; set; }

    public bool Result { get; set; }

    protected bool ShouldVisit => !Result;

    public void VisitAssignedExpression(ExpressionSyntax expression)
    {
        if (IsLocalReference(expression))
            Result = true;
    }

    public override void VisitIdentifierName(IdentifierNameSyntax node)
    {
        if (node.IsParentSyntaxKind(SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.AddressOfExpression)
            && IsLocalReference(node))
        {
            if (node.IsParentSyntaxKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                var methodSymbol = SemanticModel.GetSymbolInfo(node.Parent, CancellationToken).Symbol as IMethodSymbol;
                

                if (IsRefOrOut(methodSymbol.Parameters.FirstOrDefault()))
                {
                    Result = true;
                }
            }
            else if (node.IsParentSyntaxKind(SyntaxKind.AddressOfExpression))
            {
                Result = true;
            }
        }

        base.VisitIdentifierName(node);
    }

    private bool IsLocalReference(SyntaxNode node)
    {
        return node is IdentifierNameSyntax identifierName
            && IsLocalReference(identifierName);
    }

    private bool IsLocalReference(IdentifierNameSyntax identifierName)
    {
        return Identifiers.TryGetValue(identifierName.Identifier.ValueText, out ILocalSymbol symbol)
            && SymbolEqualityComparer.Default.Equals(symbol, SemanticModel.GetSymbolInfo(identifierName, CancellationToken).Symbol);
    }

    public static MakeLocalVariableConstantWalker GetInstance()
    {
        var walker = _cachedInstance;

        if (walker is not null)
        {
            // Debug.Assert(walker.Identifiers.Count == 0);
            // Debug.Assert(!walker.Result);
            _cachedInstance = null;
            return walker;
        }

        return new MakeLocalVariableConstantWalker();
    }

    public static void Free(MakeLocalVariableConstantWalker walker)
    {
        walker.Identifiers.Clear();
        walker.SemanticModel = null;
        walker.CancellationToken = default;
        walker.Result = false;

        _cachedInstance = walker;
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

                if (expression?.IsKind(SyntaxKind.DeclarationExpression) == false)
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

    public override void VisitPrefixUnaryExpression(PrefixUnaryExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.PreIncrementExpression, SyntaxKind.PreDecrementExpression))
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
        if (node.IsKind(SyntaxKind.PostIncrementExpression, SyntaxKind.PostDecrementExpression))
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

                    if (expression?.IsKind(SyntaxKind.DeclarationExpression) == false)
                        VisitAssignedExpression(expression);

                    break;
                }
        }

        base.VisitArgument(node);
    }
}