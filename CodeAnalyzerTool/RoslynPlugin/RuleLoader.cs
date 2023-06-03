using System.Reflection;
using CodeAnalyzerTool.API.ConfigModel;
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
        var rulePaths = new List<string>();
        var ruleConfigs = _pluginConfig.Rules.Where(r => GetNamesOfEnabledRulesFromConfig().Contains(r.RuleName));
        
        //todo: Do this higher up (in CodeAnalyzerTool) => Refactor: make a Rule Resolver
        foreach (var ruleConfig in ruleConfigs)
        {
            var externalRules =
                Path.Combine(_workingDir, _pluginsPath, _pluginConfig.PluginName, StringResources.RULES_FOLDER_NAME, ruleConfig.RuleName);

            if (Directory.Exists(externalRules))
                rulePaths.AddRange(Directory.GetFiles(externalRules, StringResources.EXTERNAL_RULE_SEARCH_PATTERN));
        }

        return rulePaths.Select(Assembly.LoadFrom)
            .SelectMany(ruleActivator.ActivateRulesFromAssembly);
    }

    private IEnumerable<RoslynRule> LoadInternalRules(RuleActivator ruleActivator)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(ruleActivator.ActivateRulesFromAssembly);
    }
}