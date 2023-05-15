using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.util;
using Serilog;

namespace CodeAnalyzerTool;

public class Program
{
    public static async Task Main()
    {
        try
        {
            LogHelper.InitLogging();
            Log.Information("Generating JSON.NET schema");
            await SchemaGenerator.GenerateSchema();
            Log.Information("Reading CAT Config file");
            var configReader = new ConfigReader();
            var globalConfig = await configReader.ReadAsync();

            // todo - maybe dependency injection - or (register all that implements interface)
            var epl = new ExternalPluginLoader(globalConfig);
            var bipl = new BuiltinPluginLoader(globalConfig);
            var plc = new PluginLoaderComposite();
            plc.AddPluginLoader(epl);
            plc.AddPluginLoader(bipl);
            var loadedPlugins = plc.LoadPlugins();

            var pluginRunner = new PluginRunner();
            var ruleViolations = await pluginRunner.RunPlugins(loadedPlugins, globalConfig.PluginsPath);

            // var pluginLoader = new PluginLoader(globalConfig);
            // var analysisResults = await pluginLoader.LoadAndRunPlugins();
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