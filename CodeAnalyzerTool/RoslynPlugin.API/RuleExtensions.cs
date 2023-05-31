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
    
    public static TSymbol GetEnclosingSymbol<TSymbol>(
        this SemanticModel semanticModel,
        int position,
        CancellationToken cancellationToken = default) where TSymbol : ISymbol
    {
        if (semanticModel is null)
            throw new ArgumentNullException(nameof(semanticModel));

        ISymbol symbol = semanticModel.GetEnclosingSymbol(position, cancellationToken);

        while (symbol is not null)
        {
            if (symbol is TSymbol tsymbol)
                return tsymbol;

            symbol = symbol.ContainingSymbol;
        }

        return default;
    }
    
    public static INamedTypeSymbol GetEnclosingNamedType(
        this SemanticModel semanticModel,
        int position,
        CancellationToken cancellationToken = default)
    {
        return GetEnclosingSymbol<INamedTypeSymbol>(semanticModel, position, cancellationToken);
    }
    
    public static bool EqualsOrInheritsFrom(this ITypeSymbol type, ITypeSymbol baseType, bool includeInterfaces = false)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        return SymbolEqualityComparer.Default.Equals(type, baseType)
               || InheritsFrom(type, baseType, includeInterfaces);
    }
    
    
    public static bool InheritsFrom(this ITypeSymbol type, ITypeSymbol baseType, bool includeInterfaces = false)
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));

        if (baseType is null)
            return false;

        INamedTypeSymbol t = type.BaseType;

        while (t is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(t.OriginalDefinition, baseType))
                return true;

            t = t.BaseType;
        }

        if (includeInterfaces
            && baseType.TypeKind == TypeKind.Interface)
        {
            foreach (INamedTypeSymbol interfaceType in type.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(interfaceType.OriginalDefinition, baseType))
                    return true;
            }
        }

        return false;
    }
}