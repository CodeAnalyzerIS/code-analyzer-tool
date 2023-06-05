using System.Collections.Immutable;
using CodeAnalyzerTool.API;
using Microsoft.CodeAnalysis;
using Location = CodeAnalyzerTool.API.Location;

namespace RoslynPlugin;

public static class DiagnosticConverter
{
    public static IEnumerable<RuleViolation> ConvertDiagnostics(ImmutableArray<Diagnostic> diagnostics)
    {
        return diagnostics.Select(ConvertDiagnostic).ToList();
    }

    private static RuleViolation ConvertDiagnostic(Diagnostic diagnostic)
    {
        var rule = ConvertDiagnosticToRule(diagnostic);
        var location = ConvertDiagnosticToLocation(diagnostic);
        var severity = ConvertDiagnosticSeverity(diagnostic.Severity);
        var result = ConvertDiagnosticToRuleViolation(diagnostic, rule, location, severity);

        return result;
    }

    private static Rule ConvertDiagnosticToRule(Diagnostic diagnostic)
    {
        var rule = new Rule(
            ruleName: diagnostic.Descriptor.Id,
            title: diagnostic.Descriptor.Title.ToString(),
            category: diagnostic.Descriptor.Category,
            defaultSeverity: ConvertDiagnosticSeverity(diagnostic.DefaultSeverity),
            description: diagnostic.Descriptor.Description.ToString(),
            isEnabledByDefault: diagnostic.Descriptor.IsEnabledByDefault,
            pluginName: StringResources.PLUGIN_NAME,
            targetLanguage: StringResources.TARGET_LANGUAGE
        );
        var properties = diagnostic.Properties;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract => doesn't make sense because type is 'ImmutableDictionary<string,string?>?'
        if (properties is not null)
        {
            properties.TryGetValue(StringResources.CODE_EXAMPLE_KEY, out string? codeExample);
            rule.CodeExample = codeExample;
            properties.TryGetValue(StringResources.CODE_EXAMPLE_FIX_KEY, out string? codeExampleFix);
            rule.CodeExampleFix = codeExampleFix;
        }
        return rule;
    }
    
    private static Location ConvertDiagnosticToLocation(Diagnostic diagnostic)
    {
        return new Location(
            path: diagnostic.Location.GetLineSpan().Path,
            startLine: diagnostic.Location.GetLineSpan().StartLinePosition.Line + 1,
            endLine: diagnostic.Location.GetLineSpan().EndLinePosition.Line + 1,
            fileExtension: StringResources.FILE_EXTENSION
        );
    }

    private static Severity ConvertDiagnosticSeverity(DiagnosticSeverity diagnosticSeverity)
    {
        return diagnosticSeverity switch
        {
            DiagnosticSeverity.Hidden => Severity.Info,
            DiagnosticSeverity.Info => Severity.Info,
            DiagnosticSeverity.Warning => Severity.Warning,
            DiagnosticSeverity.Error => Severity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(diagnosticSeverity), diagnosticSeverity, null)
        };
    }

    private static RuleViolation ConvertDiagnosticToRuleViolation(Diagnostic diagnostic, Rule rule, Location location, Severity sev)
    {
        return new RuleViolation(
            rule: rule, 
            message: diagnostic.GetMessage(), 
            location: location, 
            severity: sev);
    }
    
    public static DiagnosticSeverity ConvertSeverity(Severity severity)
    {
        return severity switch
        {
            Severity.Info => DiagnosticSeverity.Info,
            Severity.Warning => DiagnosticSeverity.Warning,
            Severity.Error => DiagnosticSeverity.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
        };
    }
}