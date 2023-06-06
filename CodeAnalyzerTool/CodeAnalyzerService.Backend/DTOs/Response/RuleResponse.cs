using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DTOs.Response;

public class RuleResponse
{
    public int Id { get; set; }
    [Required] public string RuleName { get; set; }
    [Required] public string Title { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    [Required] public string Category { get; set; }
    [Required] public string PluginName { get; set; }
    [Required] public string TargetLanguage { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public string DefaultSeverity { get; set; }
    public string? CodeExample { get; set; }
    public string? CodeExampleFix { get; set; }

    public RuleResponse(int id, string ruleName, string title, string description, string category, string pluginName,
        string targetLanguage, bool isEnabledByDefault, string defaultSeverity, string? codeExample, string? codeExampleFix)
    {
        Id = id;
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