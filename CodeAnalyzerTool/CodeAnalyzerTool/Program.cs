﻿using CodeAnalyzerTool.Api;
using CodeAnalyzerTool.Api.ConfigModel;
using CodeAnalyzerTool.PluginSystem;
using CodeAnalyzerTool.PluginSystem.Loaders;
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
            LogHelper.InitLogging(); // todo replace with DI injected loggers
            await SchemaGenerator.GenerateSchema(); // todo replace with hosted version
            var services = new ServiceCollection();

            services.AddSingleton<IPlugin, RoslynPlugin.RoslynPlugin>();
            services.AddSingleton<IPluginLoader, ExternalPluginLoader>();
            services.AddSingleton<IPluginLoader, BuiltinPluginLoader>();
            services.AddSingleton<PluginRunner, PluginRunner>();

            var configReader = new ConfigReader();
            var globalConfig = await configReader.ReadAsync();
            services.AddSingleton<GlobalConfig>(globalConfig);

            var serviceProvider = services.BuildServiceProvider();
            var pluginRunner = serviceProvider.GetRequiredService<PluginRunner>();
            var ruleViolations = await pluginRunner.Run();
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