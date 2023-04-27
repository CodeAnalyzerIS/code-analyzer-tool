using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin.rules; 

public class TestMethodWithoutAssertionAnalyzer : DiagnosticAnalyzer{
    public override void Initialize(AnalysisContext context) {
        throw new NotImplementedException();
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }
}