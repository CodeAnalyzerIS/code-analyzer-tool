using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.PluginLoader;
using CodeAnalyzerTool.util;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace CodeAnalyzerTool;

public class Program
{
    public static async Task Main()
    {
        try
        {
            var services = new ServiceCollection();
            
            services.AddSingleton<IPlugin, RoslynPlugin.RoslynPlugin>();
            var serviceProvider = services.BuildServiceProvider();

            LogHelper.InitLogging();
            Log.Information("Generating JSON.NET schema");
            await SchemaGenerator.GenerateSchema();
            Log.Information("Reading CAT Config file");
            var configReader = new ConfigReader();
            var globalConfig = await configReader.ReadAsync();
            
            var externalPluginLoader = new ExternalPluginLoader(globalConfig);
            var builtInPlugins = serviceProvider.GetServices<IPlugin>();
            var builtinPluginLoader = new BuiltinPluginLoader(globalConfig, builtInPlugins);
            var pluginLoaderComposite = new PluginLoaderComposite();
            pluginLoaderComposite.AddPluginLoader(externalPluginLoader);
            pluginLoaderComposite.AddPluginLoader(builtinPluginLoader);
            var loadedPlugins = pluginLoaderComposite.LoadPlugins();

            var pluginRunner = new PluginRunner();
            var ruleViolations = await pluginRunner.RunPlugins(loadedPlugins, globalConfig.PluginsPath);
            
            LogHelper.LogAnalysisResults(ruleViolations);

            var projectAnalysis = new ProjectAnalysis(globalConfig.ProjectName, ruleViolations);
            // todo pass projectAnalysis to backend API (C.A.S.)
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application has encountered a fatal error: {ErrorMessage}", ex.Message);
        }
    }
}