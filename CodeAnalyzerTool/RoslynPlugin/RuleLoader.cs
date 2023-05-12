using System.Collections.Immutable;
using System.Reflection;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin_API;

namespace RoslynPlugin;

public class RuleLoader
{
    private readonly string _workingDir;
    private readonly PluginConfig _pluginConfig;
    private readonly string _pluginsPath;

    public RuleLoader(string workingDir, PluginConfig pluginConfig, string pluginsPath)
    {
        _workingDir = workingDir;
        _pluginConfig = pluginConfig;
        _pluginsPath = pluginsPath;
    }

    public ImmutableArray<DiagnosticAnalyzer> LoadRules()
    {
        var enabledRuleNames = GetNamesOfEnabledRulesFromConfig().ToList();

        var analyzers = new List<DiagnosticAnalyzer>();
        var externalAnalyzers = LoadExternalRules(enabledRuleNames);
        var internalAnalyzers = LoadInternalRules(enabledRuleNames);
        analyzers.AddRange(externalAnalyzers.ToList());
        analyzers.AddRange(internalAnalyzers.ToList());

        return analyzers.ToImmutableArray();
    }

    private IEnumerable<DiagnosticAnalyzer> LoadExternalRules(ICollection<string> enabledRuleNames)
    {
        var rules = new List<string>();
        var result = new List<DiagnosticAnalyzer>();
        foreach (var ruleConfig in _pluginConfig.Rules.Where(r => enabledRuleNames.Contains(r.RuleName)))
        {
            // todo maybe change ruleName to rulePath in config?
            var externalRules =
                Path.Combine(_workingDir, _pluginsPath, _pluginConfig.PluginName, StringResources.RulesFolderName, ruleConfig.RuleName);

            if (Directory.Exists(externalRules))
                rules.AddRange(Directory.GetFiles(externalRules, StringResources.ExternalRuleSearchPattern));
        }

        foreach (var rulePath in rules)
        {
            var a = Assembly.LoadFrom(rulePath);
            var analyzers = LoadAnalyzersFromAssembly(a, enabledRuleNames);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result;
    }

    private IEnumerable<DiagnosticAnalyzer> LoadInternalRules(ICollection<string> enabledRuleNames)
    {
        var result = new List<DiagnosticAnalyzer>();

        foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
        {
            var analyzers = LoadAnalyzersFromAssembly(a, enabledRuleNames);

            if (analyzers.Any())
                result.AddRange(analyzers.Where(analyzer => analyzer != null)!);
        }

        return result;
    }
    
    private List<DiagnosticAnalyzer?> LoadAnalyzersFromAssembly(Assembly assembly,
        ICollection<string> enabledNames)
    {
        var roslynRules = assembly.GetExportedTypes()
            .Where(type => typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as RoslynRule)
            .Where(r => r?.DiagnosticId != null && enabledNames.Contains(r.DiagnosticId)).ToList();
        
        return SetRuleConfiguration(roslynRules)
            .Select(r => r as DiagnosticAnalyzer).ToList();
    }

    private IEnumerable<string> GetNamesOfEnabledRulesFromConfig()
    {
        return _pluginConfig.Rules.Where(r => r.Enabled).Select(r => r.RuleName);
    }

    private DiagnosticSeverity GetSeverityFromRuleConfig(string? ruleName)
    {
        var ruleConfig = _pluginConfig.Rules.Single(r => r.RuleName.Equals(ruleName));
        return DiagnosticConverter.ConvertSeverity(ruleConfig.Severity);
    }

    private Dictionary<string, string> GetOptionsFromRuleConfig(string? ruleName)
    {
        var ruleConfig = _pluginConfig.Rules.Single(r => r.RuleName.Equals(ruleName));
        return ruleConfig.Options;
    }

    private List<RoslynRule?> SetRuleConfiguration(IList<RoslynRule?> rules)
    {
        foreach (var rule in rules)
        {
            if (rule == null) continue;
            rule.Severity = GetSeverityFromRuleConfig(rule.DiagnosticId);
            rule.Options = GetOptionsFromRuleConfig(rule.DiagnosticId);
        }

        return rules.ToList();
    }
}