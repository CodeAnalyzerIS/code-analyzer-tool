using System.Reflection;

namespace CodeAnalyzerTool;

public static class ConfigTemplateGenerator
{
    internal static async Task GenerateConfigTemplate()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "main";
        var projectName = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar).Last();
        var template = $@"{{
    ""$schema"": ""https://raw.githubusercontent.com/CodeAnalyzerIS/code-analyzer-tool/{version}/CATSchema.json"",
    ""projectName"": ""{projectName}"",
    ""plugins"": []
}}";
        await File.WriteAllTextAsync(StringResources.CONFIG_FILE_NAME, template);
    }
}