using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Api;

public class Rule
{
    [Required] public string Id { get; set; }
    [Required] public string Title { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    [Required] public string Category { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public Severity DefaultSeverity { get; set; }

    public Rule(string id, string title, string description, string category, bool isEnabledByDefault,
        Severity defaultSeverity)
    {
        Id = id;
        Title = title;
        Description = description;
        Category = category;
        IsEnabledByDefault = isEnabledByDefault;
        DefaultSeverity = defaultSeverity;
    }
}