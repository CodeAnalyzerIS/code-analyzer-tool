using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin.API;

public abstract class RoslynRule : DiagnosticAnalyzer
{
    public abstract string RuleName { get; }
    public abstract DiagnosticSeverity Severity { get; set; }
    public abstract Dictionary<string, string> Options { get; set; }
}