using System.Reflection;

namespace CodeAnalyzerTool;

public static class ConfigTemplateGenerator
{
    internal static async Task GenerateConfigTemplate()
    {
        var projectName = Directory.GetCurrentDirectory().Split(Path.DirectorySeparatorChar).Last();
        var template = $@"{{
    ""$schema"": ""https://raw.githubusercontent.com/CodeAnalyzerIS/code-analyzer-tool/main/CATSchema.json"",
    ""projectName"": ""{projectName}"",
    ""plugins"": []
}}";
        await File.WriteAllTextAsync(StringResources.CONFIG_FILE_NAME, template);
    }
}