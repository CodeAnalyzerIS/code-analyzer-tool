using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin; 

public static class RuleLoader {
    public static ImmutableArray<DiagnosticAnalyzer> LoadRules(string workingDir) {
        var externalRules = Path.Combine(workingDir, "CAT/Roslyn/rules");
        var rules = Array.Empty<string>();
        
        if (Directory.Exists(externalRules)) {
            rules = Directory.GetFiles(externalRules, "*.dll");
        }
        
        var internalRulesFolder = Path.Combine(AppContext.BaseDirectory, "rules");
        var internalRules = Directory.GetFiles(internalRulesFolder, "*.cs");
        internalRules.CopyTo(rules, rules.Length);
            
        var result = new List<DiagnosticAnalyzer>();
        foreach (var rulePath in rules) {
            var analyzers = LoadAnalyzersFromAssembly(rulePath);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result.ToImmutableArray();
    }

    private static List<DiagnosticAnalyzer?> LoadAnalyzersFromAssembly(string path) {
        var assembly = Assembly.LoadFrom(path);
        return assembly.GetExportedTypes()
            .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as DiagnosticAnalyzer).ToList();
    }
}