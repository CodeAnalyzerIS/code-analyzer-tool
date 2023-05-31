using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class UnnecessaryTypeCastRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.UNNECESSARY_TYPE_CAST_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string CATEGORY = RuleCategories.PERFORMANCE;
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.UnnecessaryTypeCastTitle), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.UnnecessaryTypeCastMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.UnnecessaryTypeCastDescription), Resources.ResourceManager, typeof(Resources));

    public UnnecessaryTypeCastRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Warning;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY, Severity, true, Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeTypeCastExpression, SyntaxKind.CastExpression);
    }

    private void AnalyzeTypeCastExpression(SyntaxNodeAnalysisContext context)
    { 
        var castExpression = (CastExpressionSyntax)context.Node;
        if (castExpression.ContainsDiagnostics) return;
        TypeSyntax type = castExpression.Type;
        ExpressionSyntax expression = castExpression.Expression;
        SemanticModel semanticModel = context.SemanticModel;
        CancellationToken cancellationToken = context.CancellationToken;

        ITypeSymbol? typeSymbol = semanticModel.GetTypeInfo(type, cancellationToken).Type;
        if (typeSymbol is null || typeSymbol.Kind == SymbolKind.ErrorType) return;

        ITypeSymbol? expressionTypeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).Type;

        if (expressionTypeSymbol is null || typeSymbol.Kind == SymbolKind.ErrorType)
            return;

        if (expressionTypeSymbol.TypeKind == TypeKind.Interface)
            return;

        if (expressionTypeSymbol.SpecialType == SpecialType.System_Object
            || expressionTypeSymbol.TypeKind == TypeKind.Dynamic
            || typeSymbol.TypeKind != TypeKind.Interface)
        {
            if (!typeSymbol.EqualsOrInheritsFrom(expressionTypeSymbol, includeInterfaces: true))
                return;
        }
        
        
        if (castExpression.Parent is not ParenthesizedExpressionSyntax parenthesizedExpression 
            || parenthesizedExpression.Parent is null) return;
        
        ExpressionSyntax accessedExpression = GetAccessedExpression(parenthesizedExpression.Parent);
        ISymbol? accessedSymbol = semanticModel.GetSymbolInfo(accessedExpression, cancellationToken).Symbol;
        INamedTypeSymbol? containingType = accessedSymbol?.ContainingType;

        if (containingType is null) return;

        if (typeSymbol.TypeKind == TypeKind.Interface)
        {
            if (accessedSymbol.IsAbstract)
            {
                if (!CheckExplicitImplementation(expressionTypeSymbol, accessedSymbol))
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
        else
        {
            if (!CheckAccessibility(expressionTypeSymbol.OriginalDefinition, accessedSymbol, expression.SpanStart, semanticModel, cancellationToken))
                return;

            if (!expressionTypeSymbol.EqualsOrInheritsFrom(containingType, includeInterfaces: true))
                return;
        }

        var diagnostic = Diagnostic.Create(_rule, castExpression.GetLocation(), Severity);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool CheckExplicitImplementation(ITypeSymbol typeSymbol, ISymbol symbol)
    {
        ISymbol implementation = typeSymbol.FindImplementationForInterfaceMember(symbol);

        if (implementation is null)
            return false;

        switch (implementation.Kind)
        {
            case SymbolKind.Property:
                {
                    foreach (IPropertySymbol propertySymbol in ((IPropertySymbol)implementation).ExplicitInterfaceImplementations)
                    {
                        if (SymbolEqualityComparer.Default.Equals(propertySymbol.OriginalDefinition, symbol.OriginalDefinition))
                            return false;
                    }

                    break;
                }
            case SymbolKind.Method:
                {
                    foreach (IMethodSymbol methodSymbol in ((IMethodSymbol)implementation).ExplicitInterfaceImplementations)
                    {
                        if (SymbolEqualityComparer.Default.Equals(methodSymbol.OriginalDefinition, symbol.OriginalDefinition))
                            return false;
                    }

                    break;
                }
            default:
                {
                    return false;
                }
        }

        return true;
    }

    private static bool CheckAccessibility(
        ITypeSymbol expressionTypeSymbol,
        ISymbol accessedSymbol,
        int position,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        Accessibility accessibility = accessedSymbol.DeclaredAccessibility;

        if (accessibility == Accessibility.Protected
            || accessibility == Accessibility.ProtectedAndInternal)
        {
            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            while (containingType is not null)
            {
                if (SymbolEqualityComparer.Default.Equals(containingType, expressionTypeSymbol))
                    return true;

                containingType = containingType.ContainingType;
            }

            return false;
        }
        else if (accessibility == Accessibility.ProtectedOrInternal)
        {
            INamedTypeSymbol containingType = semanticModel.GetEnclosingNamedType(position, cancellationToken);

            if (SymbolEqualityComparer.Default.Equals(containingType?.ContainingAssembly, expressionTypeSymbol.ContainingAssembly))
                return true;

            while (containingType is not null)
            {
                if (SymbolEqualityComparer.Default.Equals(containingType, expressionTypeSymbol))
                    return true;

                containingType = containingType.ContainingType;
            }

            return false;
        }

        return true;
    }

    private static ExpressionSyntax GetAccessedExpression(SyntaxNode node)
    {
        switch (node?.Kind())
        {
            case SyntaxKind.SimpleMemberAccessExpression:
            case SyntaxKind.ElementAccessExpression:
                return (ExpressionSyntax)node;
            case SyntaxKind.ConditionalAccessExpression:
                return ((ConditionalAccessExpressionSyntax)node).WhenNotNull;
            default:
                return null;
        }
    }
}