using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Rule
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
    public ICollection<RuleViolation> RuleViolations { get; set; }

    public Rule(string ruleName, string title, string description, string category, string pluginName,
        string targetLanguage, bool isEnabledByDefault, string defaultSeverity, 
        string? codeExample = null, string? codeExampleFix = null)
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
        RuleViolations = new List<RuleViolation>();
    }
}