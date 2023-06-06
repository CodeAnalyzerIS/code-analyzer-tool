namespace CodeAnalyzerService.Backend.DTOs.Response;

public class RuleDetailsResponse
{
    public int Id { get; set; }
    public string RuleName { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public string PluginName { get; set; }
    public string TargetLanguage { get; set; }
    public string DefaultSeverity { get; set; }
    public string? CodeExample { get; set; }
    public string? CodeExampleFix { get; set; }

    public RuleDetailsResponse(int id, string ruleName, string title, string description, string category, 
        string pluginName, string targetLanguage, string defaultSeverity, string? codeExample, string? codeExampleFix)
    {
        Id = id;
        RuleName = ruleName;
        Title = title;
        Description = description;
        Category = category;
        PluginName = pluginName;
        TargetLanguage = targetLanguage;
        DefaultSeverity = defaultSeverity;
        CodeExample = codeExample;
        CodeExampleFix = codeExampleFix;
    }
}