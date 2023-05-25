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

        var location = new Location(
            path: diagnostic.Location.GetLineSpan().Path,
            startLine: diagnostic.Location.GetLineSpan().StartLinePosition.Line,
            endLine: diagnostic.Location.GetLineSpan().EndLinePosition.Line,
            fileExtension: StringResources.FILE_EXTENSION
        );

        var sev = ConvertDiagnosticSeverity(diagnostic.Severity);
        var result = new RuleViolation(
            rule: rule, 
            message: diagnostic.GetMessage(), 
            location: location, 
            severity: sev);

        return result;
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