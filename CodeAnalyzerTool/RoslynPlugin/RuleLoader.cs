using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin;

public static class RuleLoader
{
    public static ImmutableArray<DiagnosticAnalyzer> LoadRules(string workingDir)
    {
        var analyzers = new List<DiagnosticAnalyzer>();
        var externalAnalyzers = LoadExternalRules(workingDir);
        var internalAnalyzers = LoadInternalRules();
        analyzers.AddRange(externalAnalyzers.ToList());
        analyzers.AddRange(internalAnalyzers.ToList());

        return analyzers.ToImmutableArray();
    }

    private static IEnumerable<DiagnosticAnalyzer> LoadExternalRules(string workingDir)
    {
        //TODO: Make configurable with config file
        var externalRules = Path.Combine(workingDir, "CAT/Roslyn/rules");
        var rules = Array.Empty<string>();
        var result = new List<DiagnosticAnalyzer>();

        if (Directory.Exists(externalRules))
            rules = Directory.GetFiles(externalRules, "*.dll");

        foreach (var rulePath in rules)
        {
            var a = Assembly.LoadFrom(rulePath);
            var analyzers = LoadAnalyzersFromAssembly(a);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result;
    }

    private static IEnumerable<DiagnosticAnalyzer> LoadInternalRules()
    {
        var result = new List<DiagnosticAnalyzer>();

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            var analyzers = LoadAnalyzersFromAssembly(a);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result;
    }

    private static List<DiagnosticAnalyzer?> LoadAnalyzersFromAssembly(Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as DiagnosticAnalyzer).ToList();
    }
}