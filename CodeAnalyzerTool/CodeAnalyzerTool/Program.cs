using CAT_API;
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
            var globalConfig = await ConfigReader.ReadAsync();

            var pluginLoader = new PluginLoader(globalConfig);
            var analysisResults = await pluginLoader.LoadAndRunPlugins();
            LogHelper.LogAnalysisResults(analysisResults);

            var projectAnalysis = new ProjectAnalysis(globalConfig.ProjectId, analysisResults);
            // todo pass projectAnalysis to backend API (C.A.S.)
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application has encountered a fatal error: {ErrorMessage}", ex.Message);
        }
    }
}