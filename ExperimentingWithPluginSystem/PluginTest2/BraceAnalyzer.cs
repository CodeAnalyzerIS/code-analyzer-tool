using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace PluginTest2;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BraceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BraceAnalyzer";

    // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
    // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Localizing%20Analyzers.md for more on localization
    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.BraceAnalyzerTitle),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat =
        new LocalizableResourceString(nameof(Resources.BraceAnalyzerMessageFormat), Resources.ResourceManager,
            typeof(Resources));

    private static readonly LocalizableString Description =
        new LocalizableResourceString(nameof(Resources.BraceAnalyzerDescription), Resources.ResourceManager,
            typeof(Resources));

    private const string Category = "Naming";

    private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat,
        Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get { return ImmutableArray.Create(Rule); }
    }

    public override void Initialize(AnalysisContext context)
    {
        // context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        // context.EnableConcurrentExecution();
        //
        // // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
        // // See https://github.com/dotnet/roslyn/blob/main/docs/analyzers/Analyzer%20Actions%20Semantics.md for more information
        // context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);
        context.RegisterSyntaxTreeAction(syntaxTreeContext =>
        {
            // Iterate through all statements in the tree
            var root = syntaxTreeContext.Tree.GetRoot(syntaxTreeContext.CancellationToken);
            foreach (var statement in root.DescendantNodes().OfType<StatementSyntax>())
            {
                //skip analyzing block statements
                if (statement is BlockSyntax)
                {
                    continue;
                }

                // Report issues for all statements that are nested within a statement
                // but not a block statement
                if (statement.Parent is StatementSyntax && !(statement.Parent is BlockSyntax))
                {
                    var diagnostic = Diagnostic.Create(Rule, statement.GetFirstToken().GetLocation());
                    syntaxTreeContext.ReportDiagnostic(diagnostic);
                }
            }
        });
    }
}