using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin.rules;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class MakeLocalVariableConstantRule : RoslynRule
{
    public sealed override string RuleName => RuleNames.MAKE_LOCAL_VARIABLE_CONSTANT_RULE;
    public sealed override DiagnosticSeverity Severity { get; set; }
    public sealed override Dictionary<string, string> Options { get; set; }
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
    private const string CATEGORY = RuleCategories.PERFORMANCE;
    private readonly DiagnosticDescriptor _rule;

    private static readonly LocalizableString Title = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantTitle), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString MessageFormat = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantMessageFormat), Resources.ResourceManager, typeof(Resources));

    private static readonly LocalizableString Description = new LocalizableResourceString(
        nameof(Resources.MakeLocalVariableConstantDescription), Resources.ResourceManager, typeof(Resources));

    public MakeLocalVariableConstantRule()
    {
        Options = new Dictionary<string, string>();
        Severity = DiagnosticSeverity.Info;
        _rule = new DiagnosticDescriptor(RuleName, Title, MessageFormat, CATEGORY, Severity, true, Description);
        SupportedDiagnostics = ImmutableArray.Create(_rule);
    }

    public override void Initialize(AnalysisContext context)
    {
        throw new NotImplementedException();
    }
}