using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynPlugin.rules;

public static class RuleExtensions
{
    public static bool IsParentSyntaxKind(this SyntaxNode node, SyntaxKind kind)
    {
        return node.Parent?.RawKind == (int)kind;
    }
    
    public static bool IsParentSyntaxKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
    {
        return IsParentSyntaxKind(node, kind1) || IsParentSyntaxKind(node, kind2);
    }
    
    public static bool IsKind(this SyntaxToken token, SyntaxKind kind)
    {
        return token.RawKind == (int)kind;
    }
    
    public static bool IsKind(this SyntaxTrivia trivia, SyntaxKind kind)
    {
        return trivia.RawKind == (int)kind;
    }

    public static bool IsKind(this SyntaxNode node, SyntaxKind kind)
    {
        return node.RawKind == (int)kind;
    }
    
    public static bool IsKind(this SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
    {
        return node.IsKind(kind1) || node.IsKind(kind2);
    }
    
    public static ExpressionSyntax WalkDownParentheses(this ExpressionSyntax expression)
    {
        if (expression is null)
            throw new ArgumentNullException(nameof(expression));

        while (expression.Kind() == SyntaxKind.ParenthesizedExpression)
            expression = ((ParenthesizedExpressionSyntax)expression).Expression;

        return expression;
    }
}