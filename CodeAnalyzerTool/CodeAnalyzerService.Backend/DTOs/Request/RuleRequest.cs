using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DTOs.Request;

public class RuleRequest
{
    [Required] public string RuleName { get; set; }
    [Required] public string Title { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    [Required] public string Category { get; set; }
    [Required] public string PluginName { get; set; }
    [Required] public string TargetLanguage { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public string DefaultSeverity { get; set; }

    public RuleRequest(string ruleName, string title, string description, string category, string pluginName,
        string targetLanguage, bool isEnabledByDefault, string defaultSeverity)
    {
        RuleName = ruleName;
        Title = title;
        Description = description;
        Category = category;
        IsEnabledByDefault = isEnabledByDefault;
        DefaultSeverity = defaultSeverity;
        PluginName = pluginName;
        TargetLanguage = targetLanguage;
    }
}