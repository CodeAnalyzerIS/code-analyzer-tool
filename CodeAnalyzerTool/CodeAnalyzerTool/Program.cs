using CodeAnalyzerTool.API;
using CodeAnalyzerTool.PluginSystem;
using CodeAnalyzerTool.PluginSystem.Loaders;
using CodeAnalyzerTool.util;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace CodeAnalyzerTool;

public class Program
{
    public static async Task Main(string[] args)
    {
        try
        {
            if (args.Contains("--generate-schema"))
            {
                await SchemaGenerator.GenerateSchema();
                return;
            }

            if (args.Contains("--init"))
            {
                await ConfigTemplateGenerator.GenerateConfigTemplate();
                return;
            }

            LogHelper.InitLogging(); // todo replace with DI injected loggers
            var serviceProvider = await CreateServiceProvider();
            await Run(serviceProvider);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application has encountered a fatal error: {ErrorMessage}", ex.Message);
        }
    }

    private static async Task<ServiceProvider> CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IPlugin, RoslynPlugin.RoslynPlugin>();
        services.AddSingleton<IPluginLoader, ExternalPluginLoader>();
        services.AddSingleton<IPluginLoader, BuiltinPluginLoader>();
        services.AddSingleton<PluginRunner, PluginRunner>();
        services.AddSingleton(new HttpClient());
        services.AddSingleton<AnalysisSender, AnalysisSender>();
        
        var configReader = new ConfigReader();
        var globalConfig = await configReader.ReadAsync();
        services.AddSingleton(globalConfig);
        return services.BuildServiceProvider();
    }

    private static async Task Run(IServiceProvider serviceProvider)
    {
        var pluginRunner = serviceProvider.GetRequiredService<PluginRunner>();
        var ruleViolations = await pluginRunner.Run();
        LogHelper.LogAnalysisResults(ruleViolations);
            
        var analysisSender = serviceProvider.GetRequiredService<AnalysisSender>();
        await analysisSender.Send(ruleViolations);
    }
}