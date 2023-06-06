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
    public sealed override string CodeExample => @"class ExampleBaseClass
{
    void Main()
    {
        var a = new ExampleBaseClass();

        var i = ((ExampleClass) a).ExampleProperty;
    }

    public int ExampleProperty { get; set; }
}

class ExampleClass : ExampleBaseClass
{
}";

    public sealed override string CodeExampleFix => @"class ExampleBaseClass
{
    void Main()
    {
        var a = new ExampleBaseClass();

        var i = a.ExampleProperty;
    }

    public int ExampleProperty { get; set; }
}

class ExampleClass : ExampleBaseClass
{
}";
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
                if (!CheckExplicitInterfaceImplementation(expressionTypeSymbol, accessSymbol)) return;
            }
            else return; // when accessing default interface implementation
        }
        else
        {
            if (!CheckAccessibleWithoutTypeCast(expressionTypeSymbol.OriginalDefinition, accessSymbol, expression.SpanStart, semanticModel, cancellationToken))
                return;

            if (!IsTypeEqualOrInheritsFrom(expressionTypeSymbol, containingType))
                return;
        }
        
        var props = new Dictionary<string, string?>
        {
            {StringResources.CODE_EXAMPLE_KEY, CodeExample},
            {StringResources.CODE_EXAMPLE_FIX_KEY, CodeExampleFix }
        };

        var diagnostic = Diagnostic.Create(_rule, castExpressionSyntax.GetLocation(), effectiveSeverity: Severity, 
            null, props.ToImmutableDictionary());
        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsTypeEqualOrInheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
    {
        return SymbolEqualityComparer.Default.Equals(type, baseType) || DoesTypeInheritFrom(type, baseType);
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
    
    private static bool CheckExplicitInterfaceImplementation(ITypeSymbol typeSymbol, ISymbol symbol)
    {
        var implementation = typeSymbol.FindImplementationForInterfaceMember(symbol);
        if (implementation is null) return false;

        switch (implementation.Kind)
        {
            case SymbolKind.Method:
            {
                var implAsMethod = (IMethodSymbol)implementation;
                if (implAsMethod.ExplicitInterfaceImplementations.Length < 1) return true;
                var methodSymbol = implAsMethod.ExplicitInterfaceImplementations[0];
                if (SymbolEqualityComparer.Default.Equals(methodSymbol.OriginalDefinition, symbol.OriginalDefinition))
                    return false;

                break;
            }
            case SymbolKind.Property:
                {
                    var implAsProp = (IPropertySymbol)implementation;
                    if (implAsProp.ExplicitInterfaceImplementations.Length < 1) return true;
                    var propertySymbol = implAsProp.ExplicitInterfaceImplementations[0];
                        if (SymbolEqualityComparer.Default.Equals(propertySymbol.OriginalDefinition, symbol.OriginalDefinition))
                            return false;
                    
                    break;
                }
            default:
                {
                    return false;
                }
        }

        return true;
    }

    private static bool CheckAccessibleWithoutTypeCast(
        ITypeSymbol expressionType,
        ISymbol accessSymbol,
        int position,
        SemanticModel semanticModel,
        CancellationToken ct)
    {
        var  accessLevel = accessSymbol.DeclaredAccessibility;

        if (accessLevel is Accessibility.Protected or Accessibility.ProtectedAndInternal)
        {
            INamedTypeSymbol? typecastType = GetOuterMostType(semanticModel, position, ct);
            return IsEqualOrNestedInType(typecastType, expressionType);
        }
        else if (accessLevel == Accessibility.ProtectedOrInternal)
        {
            INamedTypeSymbol? containingType = GetOuterMostType(semanticModel, position, ct);

            if (SymbolEqualityComparer.Default.Equals(containingType?.ContainingAssembly, expressionType.ContainingAssembly))
                return true;

            return IsEqualOrNestedInType(containingType, expressionType);
        }

        return true;
    }
    
    // if the typecast type is a nested class/ type then get its outer/ enclosing type 
    // otherwise just gets the type
    private static INamedTypeSymbol? GetOuterMostType(SemanticModel semanticModel, int position, CancellationToken ct)
    {
        var symbol = semanticModel.GetEnclosingSymbol(position, ct);

        while (symbol is not null)
        {
            if (symbol is INamedTypeSymbol symbolAsNamedType) return symbolAsNamedType;

            symbol = symbol.ContainingSymbol;
        }

        return null;
    }

    private static bool IsEqualOrNestedInType(INamedTypeSymbol? type, ITypeSymbol outerMostType)
    {
        while (type is not null)
        {
            if (SymbolEqualityComparer.Default.Equals(type, outerMostType)) return true;

            type = type.ContainingType;
        }

        return false;
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