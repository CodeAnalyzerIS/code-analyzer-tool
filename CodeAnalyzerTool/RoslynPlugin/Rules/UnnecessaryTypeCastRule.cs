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
        var castExpressionSyntax = (CastExpressionSyntax)context.Node;
        if (castExpressionSyntax.ContainsDiagnostics) return;
        var castType = castExpressionSyntax.Type;
        var expression = castExpressionSyntax.Expression;
        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        // type of the typecast (which the expression gets cast into)
        var castTypeSymbol = semanticModel.GetTypeInfo(castType, cancellationToken).Type;
        if (castTypeSymbol is null) return;

        // expression that gets typecast
        var expressionTypeSymbol = semanticModel.GetTypeInfo(expression, cancellationToken).Type;
        if (expressionTypeSymbol is null) return;

        // check if castType equals or inherits from the type of the expression
        if (castTypeSymbol.TypeKind != TypeKind.Interface && 
           !IsTypeEqualOrInheritsFrom(castTypeSymbol, expressionTypeSymbol)) return;
        
        var gotAccessSymbol = TryGetAccessSymbol(semanticModel, castExpressionSyntax, cancellationToken, out ISymbol? accessSymbol);
        if (!gotAccessSymbol) return;
        var containingType = accessSymbol!.ContainingType;
        if (containingType is null) return;

        if (castTypeSymbol.TypeKind == TypeKind.Interface)
        {
            if (accessSymbol.IsAbstract)
            {
                if (!CheckExplicitImplementation(expressionTypeSymbol, accessSymbol)) return;
            }
            else return; // when accessing default interface implementation
        }
        else
        {
            if (!CheckAccessibility(expressionTypeSymbol.OriginalDefinition, accessSymbol, expression.SpanStart, semanticModel, cancellationToken))
                return;

            if (!IsTypeEqualOrInheritsFrom(expressionTypeSymbol, containingType))
                return;
        }

        var diagnostic = Diagnostic.Create(_rule, castExpressionSyntax.GetLocation(), Severity);
        context.ReportDiagnostic(diagnostic);
    }

    private static bool TryGetAccessSymbol(SemanticModel semanticModel, SyntaxNode castExpression, CancellationToken ct, out ISymbol? accessSymbol)
    {
        var accessExpression = castExpression.Parent?.Parent; // castExpression -> parenthesizedExpression -> accessExpression
        if (accessExpression is null)
        {
            accessSymbol = null;
            return false;
        }
        
        accessSymbol = semanticModel.GetSymbolInfo(accessExpression, ct).Symbol;
        if (accessSymbol is not null) return true;
        
        // When accessExpression is a conditional expression it has to be cast to ConditionalAccessExpressionSyntax to access
        //      the WhenNotNull prop (to be able to extract the real expression from the conditional abstraction layer).
        try
        {
            var expressionWhenConditionalNotNull = ((ConditionalAccessExpressionSyntax)accessExpression).WhenNotNull;
            accessSymbol = semanticModel.GetSymbolInfo(expressionWhenConditionalNotNull, ct).Symbol;
            if (accessSymbol is null) return false;
            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private static bool CheckExplicitImplementation(ITypeSymbol typeSymbol, ISymbol symbol)
    {
        // TODO 
        var implementation = typeSymbol.FindImplementationForInterfaceMember(symbol);
        if (implementation is null) return false;

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
    
    private static bool IsTypeEqualOrInheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
    {
        return SymbolEqualityComparer.Default.Equals(type, baseType) || DoesTypeInheritFrom(type, baseType);
    }

    private static bool DoesTypeInheritFrom(ITypeSymbol type, ITypeSymbol baseType)
    {
        INamedTypeSymbol? currentType = type.BaseType;

        while (currentType is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(currentType, baseType)) return true;

            currentType = currentType.BaseType;
        }

        return false;
    }
}