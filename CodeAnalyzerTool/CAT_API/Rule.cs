namespace CAT_API;

public class Rule
{
    // Normally only possible to create AnalysisResult instance with public constructor so Id, Title, etc. cannot be null
    public string Id { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Category { get; set; } = null!;
    public bool IsEnabledByDefault { get; set; }
    public Severity DefaultSeverity { get; set; }
    
    // Only meant for Entity Framework
    private Rule() {}

    public Rule(string id, string title, string description, string category, bool isEnabledByDefault, Severity defaultSeverity)
    {
        Id = id;
        Title = title;
        Description = description;
        Category = category;
        IsEnabledByDefault = isEnabledByDefault;
        DefaultSeverity = defaultSeverity;
    }
}