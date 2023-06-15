namespace CodeAnalyzerTool.Config;

/// <summary>
/// The <c>ConfigTemplateGenerator</c> class generates a config file filled with a template.
/// </summary>
public static class ConfigTemplateGenerator
{
    /// <summary>
    /// Generates a configuration template file with default settings.
    /// The template has the schema value already set to the appropriate JSON schema for autocomplete functionality.
    /// The generated template includes the project name and an empty list of plugins.
    /// </summary>
    /// <returns>A void <c>Task</c> representing the asynchronous operation.</returns>
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