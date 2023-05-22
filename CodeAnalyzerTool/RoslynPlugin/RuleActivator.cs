using System.Reflection;
using CodeAnalyzerTool.API.ConfigModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

namespace RoslynPlugin;

public class RuleActivator
{
    private readonly PluginConfig _pluginConfig;
    private readonly IEnumerable<string> _enabledRuleNames;

    public RuleActivator(PluginConfig pluginConfig, IEnumerable<string> enabledRuleNames)
    {
        _pluginConfig = pluginConfig;
        _enabledRuleNames = enabledRuleNames;
    }
    
    public IEnumerable<RoslynRule> ActivateRulesFromAssembly(Assembly assembly)
    {
        return assembly.GetExportedTypes()
            .Where(IsValid)
            .Select(CreateInstance)
            .Where(r => r != null)
            .Select(r => r!)
            .Where(r => IsEnabled(_enabledRuleNames, r))
            .Select(SetRuleConfiguration);
    }

    private static bool IsValid(Type type)
    {
        return typeof(DiagnosticAnalyzer).IsAssignableFrom(type) && !type.IsAbstract;
    }

    private static RoslynRule? CreateInstance(Type type)
    {
        return Activator.CreateInstance(type) as RoslynRule;
    }

    
    private RoslynRule SetRuleConfiguration(RoslynRule r)
    {
        r.Severity = GetSeverityFromRuleConfig(r.RuleName);
        r.Options = GetOptionsFromRuleConfig(r.RuleName);
        return r;
    }
    
    private static bool IsEnabled(IEnumerable<string> enabledRuleNames, RoslynRule r)
    {
        return enabledRuleNames.Contains(r.RuleName);
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
}