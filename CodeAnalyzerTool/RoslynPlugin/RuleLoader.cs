using System.Collections.Immutable;
using System.Reflection;
using CAT_API;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RoslynPlugin;

public static class RuleLoader
{
    public static ImmutableArray<DiagnosticAnalyzer> LoadRules(string workingDir, PluginConfig pluginConfig)
    {
        var enabledRuleNames = EnabledRuleNames(pluginConfig);

        var analyzers = new List<DiagnosticAnalyzer>();
        // var externalAnalyzers = LoadExternalRules(workingDir);
        var internalAnalyzers = LoadInternalRules(enabledRuleNames.ToList(), pluginConfig);
        // analyzers.AddRange(externalAnalyzers.ToList());
        analyzers.AddRange(internalAnalyzers.ToList());

        return analyzers.ToImmutableArray();
    }

    // private static IEnumerable<DiagnosticAnalyzer> LoadExternalRules(string workingDir)
    // {
    //     //TODO: Make configurable with config file
    //     var externalRules = Path.Combine(workingDir, "CAT/Roslyn/rules");
    //     var rules = Array.Empty<string>();
    //     var result = new List<DiagnosticAnalyzer>();
    //
    //     if (Directory.Exists(externalRules))
    //         rules = Directory.GetFiles(externalRules, "*.dll");
    //
    //     foreach (var rulePath in rules)
    //     {
    //         var a = Assembly.LoadFrom(rulePath);
    //         var analyzers = LoadAnalyzersFromAssembly(a);
    //
    //         if (analyzers.Any())
    //             result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
    //     }
    //
    //     return result;
    // }

    private static IEnumerable<DiagnosticAnalyzer> LoadInternalRules(ICollection<string> enabledRuleNames,
        PluginConfig pluginConfig)
    {
        var result = new List<DiagnosticAnalyzer>();

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            var analyzers = LoadAnalyzersFromAssembly(a, enabledRuleNames, pluginConfig);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result;
    }

    private static List<DiagnosticAnalyzer?> LoadAnalyzersFromAssembly(Assembly assembly,
        ICollection<string> enabledNames, PluginConfig pluginConfig)
    {
        return assembly.GetExportedTypes()
            .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
            .Where(type => enabledNames.Contains(type.GetField("DiagnosticId")?.GetValue(null)))
            .Select(type => Activator.CreateInstance(type,
                    GetSeverityFromRuleConfig(type.GetField("DiagnosticId")?.GetValue(null)?.ToString(), pluginConfig),
                    GetOptionsFromRuleConfig(type.GetField("DiagnosticId")?.GetValue(null)?.ToString(), pluginConfig))
                as DiagnosticAnalyzer).ToList();
    }

    private static IEnumerable<string> EnabledRuleNames(PluginConfig pluginConfig)
    {
        return pluginConfig.Rules.Where(r => r.Enabled).Select(r => r.RuleName);
    }

    private static DiagnosticSeverity GetSeverityFromRuleConfig(string? ruleName, PluginConfig plugConf)
    {
        var ruleConfig = plugConf.Rules.Single(r => r.RuleName.Equals(ruleName));
        return ruleConfig.Severity switch
        {
            Severity.Info => DiagnosticSeverity.Info,
            Severity.Warning => DiagnosticSeverity.Warning,
            Severity.Error => DiagnosticSeverity.Error,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static Dictionary<string, string> GetOptionsFromRuleConfig(string? ruleName, PluginConfig pluginConfig)
    {
        var ruleConfig = pluginConfig.Rules.Single(r => r.RuleName.Equals(ruleName));
        return ruleConfig.Options;
    }
}