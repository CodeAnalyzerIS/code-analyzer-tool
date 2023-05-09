namespace CAT_API;

public class AnalysisResult
{
    public int Id { get; set; }
    // Normally only possible to create AnalysisResult instance with public constructor so Rule, PluginId, etc. cannot be null
    public Rule Rule { get; set; } = null!;
    public string PluginId { get; set; } = null!;
    public string Message { get; set; } = null!;
    // What (programming) language the analyzer is analysing (i.e. c#, markdown, etc.)
    public string TargetLanguage { get; set; } = null!;
    public Location Location { get; set; } = null!;
    public Severity Severity { get; set; }
    
    // Only meant for Entity Framework
    private AnalysisResult() {}

    public AnalysisResult(Rule rule, string pluginId, string message, string targetLanguage, Location location, Severity severity)
    {
        Rule = rule;
        PluginId = pluginId;
        Message = message;
        TargetLanguage = targetLanguage;
        Location = location;
        Severity = severity;
    }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Rule)}: {Rule}, {nameof(PluginId)}: {PluginId}, {nameof(Message)}: {Message}, {nameof(TargetLanguage)}: {TargetLanguage}, {nameof(Location)}: {Location}, {nameof(Severity)}: {Severity}";
    }
}