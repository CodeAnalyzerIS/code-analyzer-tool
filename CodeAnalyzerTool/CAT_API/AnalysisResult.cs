namespace CAT_API;

public class AnalysisResult
{
    public int Id { get; set; }
    public Rule Rule { get; set; }
    public string PluginId { get; set; }
    public string Message { get; set; }
    // What (programming) language the analyzer is analysing (i.e. c#, markdown, etc.)
    public string TargetLanguage { get; set; }
    public Location Location { get; set; }
    public Severity Severity { get; set; }

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