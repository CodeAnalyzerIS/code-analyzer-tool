using System.Collections.Immutable;
using System.Reflection;
using CAT_API.ConfigModel;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynPlugin.API;

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

    public IEnumerable<RoslynRule> LoadRules()
    {
        var ruleActivator = new RuleActivator(_pluginConfig, GetNamesOfEnabledRulesFromConfig());

        var rules = new List<RoslynRule>();
        rules.AddRange(LoadExternalRules(ruleActivator));
        rules.AddRange(LoadInternalRules(ruleActivator));

        return rules;
    }

    private IEnumerable<string> GetNamesOfEnabledRulesFromConfig()
    {
        return _pluginConfig.Rules.Where(r => r.Enabled).Select(r => r.RuleName);
    }
    
    private IEnumerable<RoslynRule> LoadExternalRules(RuleActivator ruleActivator)
    {
        var rules = new List<string>();
        
        //todo: Do this higher up (in CodeAnalyzerTool)
        foreach (var ruleConfig in _pluginConfig.Rules.Where(r => GetNamesOfEnabledRulesFromConfig().Contains(r.RuleName)))
        {
            // todo maybe change ruleName to rulePath in config?
            var externalRules =
                Path.Combine(_workingDir, _pluginsPath, _pluginConfig.PluginName, StringResources.RulesFolderName, ruleConfig.RuleName);

            if (Directory.Exists(externalRules))
                rules.AddRange(Directory.GetFiles(externalRules, StringResources.ExternalRuleSearchPattern));
        }

        return rules.Select(Assembly.LoadFrom)
            .SelectMany(ruleActivator.ActivateRulesFromAssembly);
    }

    private IEnumerable<RoslynRule> LoadInternalRules(RuleActivator ruleActivator)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(ruleActivator.ActivateRulesFromAssembly);
    }
}