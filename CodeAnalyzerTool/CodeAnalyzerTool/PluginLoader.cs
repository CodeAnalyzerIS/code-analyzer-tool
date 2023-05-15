using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.Json;
using CAT_API;
using CAT_API.ConfigModel;
using RoslynPlugin;
using Serilog;

namespace CodeAnalyzerTool;

public class PluginLoader
{
    private readonly GlobalConfig _globalConfig;

    public PluginLoader(GlobalConfig globalConfig)
    {
        _globalConfig = globalConfig;
    }
    
    public async Task<List<RuleViolation>> LoadAndRunPlugins()
    {
        var analysisResults = new List<RuleViolation>();
        var externalPluginResults = await RunPlugins(
            pluginsDictionary: LoadExternalPlugins(),
            pluginConfigs: GetExternalPluginConfigs().ToList(),
            pluginsPath: _globalConfig.PluginsPath);

        var builtInPluginResults = await RunPlugins(
            pluginsDictionary: LoadBuiltInPlugins(),
            pluginConfigs: GetBuiltInPluginConfigs().ToList(),
            pluginsPath: _globalConfig.PluginsPath);

        AddValidatedResults(externalPluginResults, analysisResults);
        AddValidatedResults(builtInPluginResults, analysisResults);

        return analysisResults;
    }

    private void AddValidatedResults(IEnumerable<RuleViolation> resultsToValidate,
        List<RuleViolation> listToAddResultsTo)
    {
        var validatedResults = resultsToValidate.Where(result =>
        {
            var validationResults = new List<ValidationResult>();
            var valid = Validator.TryValidateObject(result, new ValidationContext(result),
                validationResults, true);
            if (!valid)
            {
                var errorMessages = string.Join(" | ", validationResults.Select(vr => vr.ToString()));
                Log.Warning("Invalid {AnalysisResult} detected: {ErrorMessages}", nameof(RuleViolation),
                    errorMessages);
            }

            return valid;
        });
        listToAddResultsTo.AddRange(validatedResults);
    }

    private async Task<IEnumerable<RuleViolation>> RunPlugins(Dictionary<string, IPlugin> pluginsDictionary,
        ICollection<PluginConfig> pluginConfigs, string pluginsPath)
    {
        var analysisResults = new List<RuleViolation>();
        foreach (var kv in pluginsDictionary)
        {
            var pluginConfig = pluginConfigs.Single(p => p.PluginName == kv.Key);
            var pluginResults = await kv.Value.Analyze(pluginConfig, pluginsPath);
            analysisResults.AddRange(pluginResults);
        }

        return analysisResults;
    }

    private Dictionary<string, IPlugin> LoadBuiltInPlugins()
    {
        var builtInPlugins = new Dictionary<string, IPlugin>();
        foreach (var pluginConfig in GetBuiltInPluginConfigs())
        {
            Log.Information("Loading built-in plugin: {PluginName}", pluginConfig.PluginName);
            switch (pluginConfig.PluginName)
            {
                case StringResources.RoslynPluginName:
                    builtInPlugins[pluginConfig.PluginName] = new RoslynMain();
                    break;
                default:
                    Log.Error("Loading built-in plugin failed: {PluginName} is not a recognized built-in plugin",
                        pluginConfig.PluginName);
                    break;
            }
        }

        return builtInPlugins;
    }

    private Dictionary<string, IPlugin> LoadExternalPlugins()
    {
        return GetExternalPluginConfigs()
            .SelectMany(pluginConfig => LoadExternalPlugin(_globalConfig.PluginsPath, pluginConfig))
            .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private Dictionary<string, IPlugin> LoadExternalPlugin(string pluginsPath, PluginConfig config)
    {
        try
        {
            Log.Information("Loading external plugin: {PluginName}", config.PluginName);
            if (config.AssemblyName == null)
                throw new JsonException("Invalid config file: AssemblyName is a required field for external plugins.");
            var pluginAssemblyPath = Path.Combine(pluginsPath, config.FolderName, config.AssemblyName);
            Assembly pluginAssembly = LoadPlugin(pluginAssemblyPath);
            return CreateExternalPlugin(pluginAssembly, config.PluginName);
        }
        catch (Exception ex)
        {
            Log.Error("Loading external plugin failed: {ErrorMessage}", ex.Message);
            return new Dictionary<string, IPlugin>();
        }
    }

    private Assembly LoadPlugin(string assemblyPath)
    {
        var loadContext = new PluginLoadContext(assemblyPath);
        var assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(assemblyPath));
        return loadContext.LoadFromAssemblyName(assemblyName);
    }

    private Dictionary<string, IPlugin> CreateExternalPlugin(Assembly assembly, string pluginName)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!typeof(IPlugin).IsAssignableFrom(type)) continue;

            IPlugin? result = Activator.CreateInstance(type) as IPlugin;
            if (result != null)
            {
                return new Dictionary<string, IPlugin>
                {
                    [pluginName] = result
                };
            }
        }

        throw new TypeLoadException($"Can't find any type which implements IPlugin in {assembly.Location}.");
    }

    private IEnumerable<PluginConfig> GetExternalPluginConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: not null });
    }

    private IEnumerable<PluginConfig> GetBuiltInPluginConfigs()
    {
        return _globalConfig.Plugins.Where(p => p is { Enabled: true, AssemblyName: null });
    }
}