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
        var rule = new Rule()
        {
            Id = diagnostic.Descriptor.Id,
            Title = diagnostic.Descriptor.Title.ToString(),
            Category = diagnostic.Descriptor.Category,
            DefaultSeverity = ConvertSeverity(diagnostic.DefaultSeverity),
            Description = diagnostic.Descriptor.Description.ToString(),
            IsEnabledByDefault = diagnostic.Descriptor.IsEnabledByDefault
        }; // todo maybe make extension?

        var location = new Location()
        {
            Path = diagnostic.Location.GetLineSpan().Path,
            StartLine = diagnostic.Location.GetLineSpan().StartLinePosition.Line,
            EndLine = diagnostic.Location.GetLineSpan().EndLinePosition.Line,
            FileExtension = ".cs" // TODO not hardcoded
        }; // todo maybe make extension?
        
        AnalysisResult result = new AnalysisResult()
        {
            Rule = rule,
            PluginId = "Roslyn", // TODO not hardcoded
            TargetLanguage = "c#",// TODO vraag of mss betere manier (zonder Enum die het restrict vr packages)
            Location = location,
            Severity = ConvertSeverity(diagnostic.Severity), // TODO test to make sure the severity can be configured in config file
            Message = diagnostic.GetMessage()
        };

        return result;
    }

    private static Severity ConvertSeverity(DiagnosticSeverity diagnosticSeverity)
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
}