using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Sender.DTO;

public class RuleDto
{
    [Required] public string RuleName { get; set; }
    [Required] public string Title { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    [Required] public string Category { get; set; }
    [Required] public string PluginName { get; set; }
    [Required] public string TargetLanguage { get; set; }
    public string? CodeExample { get; set; }
    public string? CodeExampleFix { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public string DefaultSeverity { get; set; }

    public RuleDto(string ruleName, string title, string description, string category, string pluginName,
        string targetLanguage, bool isEnabledByDefault, string defaultSeverity, string? codeExample, string? codeExampleFix)
    {
        RuleName = ruleName;
        Title = title;
        Description = description;
        Category = category;
        IsEnabledByDefault = isEnabledByDefault;
        DefaultSeverity = defaultSeverity;
        PluginName = pluginName;
        TargetLanguage = targetLanguage;
        CodeExample = codeExample;
        CodeExampleFix = codeExampleFix;
    }
}