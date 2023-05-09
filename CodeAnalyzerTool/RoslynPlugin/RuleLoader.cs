using System.Collections.Immutable;
using System.Reflection;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin_API;

namespace RoslynPlugin;

public static class RuleLoader
{
    public static ImmutableArray<DiagnosticAnalyzer> LoadRules(string workingDir, PluginConfig pluginConfig,
        string pluginsPath)
    {
        var enabledRuleNames = EnabledRuleNames(pluginConfig).ToList();

        var analyzers = new List<DiagnosticAnalyzer>();
        var externalAnalyzers = LoadExternalRules(workingDir, enabledRuleNames, pluginConfig, pluginsPath);
        var internalAnalyzers = LoadInternalRules(enabledRuleNames, pluginConfig);
        analyzers.AddRange(externalAnalyzers.ToList());
        analyzers.AddRange(internalAnalyzers.ToList());

        return analyzers.ToImmutableArray();
    }

    private static IEnumerable<DiagnosticAnalyzer> LoadExternalRules(string workingDir,
        ICollection<string> enabledRuleNames, PluginConfig pluginConfig, string pluginsPath)
    {
        var rules = new List<string>();
        var result = new List<DiagnosticAnalyzer>();
        foreach (var ruleConfig in pluginConfig.Rules.Where(r => r.Enabled))
        {
            // todo maybe change ruleName to rulePath in config?
            var externalRules =
                Path.Combine(workingDir, pluginsPath, pluginConfig.PluginName, StringResources.RulesFolderName, ruleConfig.RuleName);

            if (Directory.Exists(externalRules))
                rules.AddRange(Directory.GetFiles(externalRules, StringResources.ExternalRuleSearchPattern));
        }

        foreach (var rulePath in rules)
        {
            var a = Assembly.LoadFrom(rulePath);
            var analyzers = LoadAnalyzersFromAssembly(a, enabledRuleNames, pluginConfig);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result;
    }

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
        var roslynRules = assembly.GetExportedTypes()
            .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as RoslynRule)
            .Where(r => r?.DiagnosticId != null && enabledNames.Contains(r.DiagnosticId)).ToList();
        return SetRuleConfiguration(roslynRules, pluginConfig)
            .Select(r => r as DiagnosticAnalyzer).ToList();
    }

    private static IEnumerable<string> EnabledRuleNames(PluginConfig pluginConfig)
    {
        return pluginConfig.Rules.Where(r => r.Enabled).Select(r => r.RuleName);
    }

    private static DiagnosticSeverity GetSeverityFromRuleConfig(string? ruleName, PluginConfig plugConf)
    {
        var ruleConfig = plugConf.Rules.Single(r => r.RuleName.Equals(ruleName));
        return DiagnosticConverter.ConvertSeverity(ruleConfig.Severity);
    }

    private static Dictionary<string, string> GetOptionsFromRuleConfig(string? ruleName, PluginConfig pluginConfig)
    {
        var ruleConfig = pluginConfig.Rules.Single(r => r.RuleName.Equals(ruleName));
        return ruleConfig.Options;
    }

    private static List<RoslynRule?> SetRuleConfiguration(IList<RoslynRule?> rules, PluginConfig pluginConfig)
    {
        foreach (var rule in rules)
        {
            if (rule == null) continue;
            rule.Severity = GetSeverityFromRuleConfig(rule.DiagnosticId, pluginConfig);
            rule.Options = GetOptionsFromRuleConfig(rule.DiagnosticId, pluginConfig);
        }

        return rules.ToList();
    }
}