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
        if (IsParentSyntaxKind(node, SyntaxKind.SimpleMemberAccessExpression, SyntaxKind.AddressOfExpression)
            && IsLocalReference(node))
        {
            if (IsParentSyntaxKind(node, SyntaxKind.SimpleMemberAccessExpression))
            {
                var methodSymbol = SemanticModel.GetSymbolInfo(node.Parent, CancellationToken).Symbol as IMethodSymbol;
                

                if (IsRefOrOut(methodSymbol.Parameters.FirstOrDefault()))
                {
                    Result = true;
                }
            }
            else if (IsParentSyntaxKind(node, SyntaxKind.AddressOfExpression))
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

    private static bool IsParentSyntaxKind(SyntaxNode node, SyntaxKind kind)
    {
        return node.Parent?.RawKind == (int)kind;
    }
    
    private static bool IsParentSyntaxKind(SyntaxNode node, SyntaxKind kind1, SyntaxKind kind2)
    {
        return IsParentSyntaxKind(node, kind1) || IsParentSyntaxKind(node, kind2);
    }
    
    private static bool IsRefOrOut(IParameterSymbol parameterSymbol)
    {
        if (parameterSymbol is null)
            throw new ArgumentNullException(nameof(parameterSymbol));

        RefKind refKind = parameterSymbol.RefKind;

        return refKind == RefKind.Ref
               || refKind == RefKind.Out;
    }
}