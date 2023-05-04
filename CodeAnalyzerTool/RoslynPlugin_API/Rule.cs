using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin_API;

public abstract class Rule : DiagnosticAnalyzer
{
    public abstract string DiagnosticId { get; }
    public abstract DiagnosticSeverity Severity { get; set; }
    public abstract Dictionary<string, string> Options { get; set; }
}