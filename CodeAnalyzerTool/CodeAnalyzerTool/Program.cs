using CodeAnalyzerTool.util;
using Serilog;

namespace CodeAnalyzerTool;

public class Program
{
    public static async Task Main()
    {
        try
        {
            // todo maybe extension method instead of toString inside Domain models?
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
            Log.Information("Generating JSON.NET schema");
            await SchemaGenerator.GenerateSchema();
            Log.Information("Reading CAT Config file");
            var globalConfig = await ConfigReader.ReadAsync();

            var analysisResults = await PluginLoader.LoadAndRunPlugins(globalConfig);
            if (analysisResults.Count > 0) analysisResults.ForEach(r => 
                Log.Write(LogHelper.SeverityToLogLevel(r.Severity), "{Message}",r.Message));
            else Log.Information("No problems found! (no analysis results)");

            // todo pass result to backend API (C.A.S.)
        }
        catch (Exception ex)
        {
            // todo fix exception handling
            Log.Fatal("Application has encountered a fatal error: {Message}",ex.Message);
        }
    }
}