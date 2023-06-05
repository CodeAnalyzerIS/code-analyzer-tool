using CodeAnalyzerTool.API;
using CodeAnalyzerTool.Config;
using CodeAnalyzerTool.PluginSystem;
using CodeAnalyzerTool.PluginSystem.Loaders;
using CodeAnalyzerTool.Sender;
using CodeAnalyzerTool.util;
using Serilog;
using Microsoft.Extensions.DependencyInjection;

namespace CodeAnalyzerTool;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            if (args.Contains("--generate-schema"))
            {
                await SchemaGenerator.GenerateSchema();
                return 0;
            }

            if (args.Contains("--init"))
            {
                await ConfigTemplateGenerator.GenerateConfigTemplate();
                return 0;
            }

            LogHelper.InitLogging(); // todo replace with DI injected loggers
            var serviceProvider = await CreateServiceProvider();
            return await Run(serviceProvider);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application has encountered a fatal error: {ErrorMessage}", ex.Message);
            return -1;
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

    private static async Task<int> Run(IServiceProvider serviceProvider)
    {
        var pluginRunner = serviceProvider.GetRequiredService<PluginRunner>();
        var ruleViolations = await pluginRunner.Run();
        LogHelper.LogAnalysisResults(ruleViolations);
            
        var analysisSender = serviceProvider.GetRequiredService<AnalysisSender>();
        await analysisSender.Send(ruleViolations);
        if (ruleViolations.Any(rv => rv.Severity == Severity.Error))
        {
            return -1;
        }

        return 0;
    }
}