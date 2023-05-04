using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace BraceAnalyzerPlugin;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class BraceAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "BraceAnalyzer";
    private const string Category = "Style";
    private readonly DiagnosticSeverity _severity;
    private readonly Dictionary<string, string> _options;

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
    
    private static readonly DiagnosticDescriptor Rule = new (DiagnosticId, Title, MessageFormat,
        Category, DiagnosticSeverity.Info, isEnabledByDefault: true, description: Description);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public BraceAnalyzer()
    {
        _options = new Dictionary<string, string>();
        _severity = DiagnosticSeverity.Warning;
    }

    public BraceAnalyzer(DiagnosticSeverity severity, Dictionary<string, string> options)
    {
        _severity = severity;
        _options = options;
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxTreeAction(AnalyzeBlockStatements);
    }

    private void AnalyzeBlockStatements(SyntaxTreeAnalysisContext ctx)
    {
        var root = ctx.Tree.GetRoot(ctx.CancellationToken);
        foreach (var statement in root.DescendantNodes().OfType<StatementSyntax>())
        {
            if (statement is BlockSyntax) continue;

            if (statement.Parent is not StatementSyntax or BlockSyntax) continue;
            var diagnostic = Diagnostic.Create(Rule, 
                statement.GetFirstToken().GetLocation(),
                effectiveSeverity: _severity,
                null,
                null);
            ctx.ReportDiagnostic(diagnostic);
        }
    }
}