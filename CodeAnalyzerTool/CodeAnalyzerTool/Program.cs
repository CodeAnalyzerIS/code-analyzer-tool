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

            var analysisResults = await PluginLoader.LoadAndRunPlugins(globalConfig);
            LogHelper.LogAnalysisResults(analysisResults);

            // todo pass result to backend API (C.A.S.)
        }
        catch (Exception ex)
        {
            Log.Fatal("Application has encountered a fatal error: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}