using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin_API;

namespace LicenseCheckRule;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class LicenceCheckRule : RoslynRule
{
    public sealed override string DiagnosticId => "LicenseCheck";
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string Category = "Convention";
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.LicenseCheckTitle),
        Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.LicenceCheckMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.LicenceCheckDescription),
        Resources.ResourceManager, typeof(Resources));

    public LicenceCheckRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Info;
        _rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, Severity,
            isEnabledByDefault: true, description: Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        
        //Maybe not with console.writeline?
        if (!Options.TryGetValue("license", out _))
        {
            Console.WriteLine("No license was provided through the config");
            return;
        }
        context.RegisterSyntaxTreeAction(CheckLicense);
    }

    private void CheckLicense(SyntaxTreeAnalysisContext ctx)
    {
        var root = ctx.Tree.GetRoot(ctx.CancellationToken);
        var firstComment = root.DescendantTrivia().FirstOrDefault(t => t.IsKind(SyntaxKind.SingleLineCommentTrivia));
        if (CheckFirstCommentForLicense(firstComment)) return;
        var diagnostic = Diagnostic.Create(_rule,
            root.GetFirstToken().GetLocation(),
            effectiveSeverity: Severity,
            null,
            null,
            Options["license"]);
        ctx.ReportDiagnostic(diagnostic);
    }

    private bool CheckFirstCommentForLicense(SyntaxTrivia firstComment)
    {
        return firstComment.SpanStart == 0 && firstComment.ToString().Contains(Options["license"]);
    }
}