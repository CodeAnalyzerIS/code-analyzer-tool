using System.Collections.Immutable;
using CAT_API;
using Microsoft.CodeAnalysis;
using Location = CAT_API.Location;

namespace RoslynPlugin;

public static class DiagnosticConverter
{
    public static IEnumerable<AnalysisResult> ConvertDiagnostics(ImmutableArray<Diagnostic> diagnostics)
    {
        return diagnostics.Select(ConvertDiagnostic).ToList();
    }

    private static AnalysisResult ConvertDiagnostic(Diagnostic diagnostic)
    {
        var rule = new Rule(
            id: diagnostic.Descriptor.Id,
            title: diagnostic.Descriptor.Title.ToString(),
            category: diagnostic.Descriptor.Category,
            defaultSeverity: ConvertDiagnosticSeverity(diagnostic.DefaultSeverity),
            description: diagnostic.Descriptor.Description.ToString(),
            isEnabledByDefault: diagnostic.Descriptor.IsEnabledByDefault
        );

        var location = new Location(
            path: diagnostic.Location.GetLineSpan().Path,
            startLine: diagnostic.Location.GetLineSpan().StartLinePosition.Line,
            endLine: diagnostic.Location.GetLineSpan().EndLinePosition.Line,
            fileExtension: StringResources.FileExtension
        );

        var sev = ConvertDiagnosticSeverity(diagnostic.Severity);
        var result = new AnalysisResult(
            rule: rule, 
            pluginId: StringResources.PluginId, 
            message: diagnostic.GetMessage(), 
            targetLanguage: StringResources.TargetLanguage, 
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