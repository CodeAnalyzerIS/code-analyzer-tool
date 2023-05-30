using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynPlugin.API;

public static class RuleExtensions
{
    public static bool IsParentOfSyntaxKind(this SyntaxNode node, SyntaxKind kind)
    {
        return node.Parent?.RawKind == (int)kind;
    }
    
    public static bool IsParentOfSyntaxKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
    {
        return IsParentOfSyntaxKind(node, kind1) || IsParentOfSyntaxKind(node, kind2);
    }

    public static bool IsOfSyntaxKind(this SyntaxNode node, SyntaxKind kind)
    {
        return node.RawKind == (int)kind;
    }
    
    public static bool IsOfSyntaxKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
    {
        return node.IsOfSyntaxKind(kind1) || node.IsOfSyntaxKind(kind2);
    }

    public static bool IsOfSyntaxKind(this SyntaxToken node, SyntaxKind kind)
    {
        return node.RawKind == (int)kind;
    }
    
    public static bool IsOfSyntaxKind(this SyntaxToken node, SyntaxKind kind1, SyntaxKind kind2)
    {
        return node.IsOfSyntaxKind(kind1) || node.IsOfSyntaxKind(kind2);
    }
    
    public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
    {
        if (expression is null) throw new ArgumentNullException(nameof(expression));

        while (expression.Kind() == SyntaxKind.ParenthesizedExpression)
            expression = ((ParenthesizedExpressionSyntax)expression).Expression;

        return expression;
    }
}