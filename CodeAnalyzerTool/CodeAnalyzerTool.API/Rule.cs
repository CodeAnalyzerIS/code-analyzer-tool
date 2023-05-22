using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.API;

public class Rule
{
    [Required] public string RuleName { get; set; }
    [Required] public string Title { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    [Required] public string Category { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public Severity DefaultSeverity { get; set; }

    public Rule(string ruleName, string title, string description, string category, bool isEnabledByDefault,
        Severity defaultSeverity)
    {
        RuleName = ruleName;
        Title = title;
        Description = description;
        Category = category;
        IsEnabledByDefault = isEnabledByDefault;
        DefaultSeverity = defaultSeverity;
    }
}