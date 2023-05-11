using System.ComponentModel.DataAnnotations;

namespace CAT_API;

public class AnalysisResult
{
    public int Id { get; set; }
    [Required] public Rule Rule { get; set; }
    [Required] public string PluginId { get; set; }
    [Required] public string Message { get; set; }
    [Required] public string TargetLanguage { get; set; }
    [Required] public Location Location { get; set; }
    public Severity Severity { get; set; }

    public AnalysisResult(Rule rule, string pluginId, string message, string targetLanguage, Location location,
        Severity severity)
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
        return
            $"[{Severity.ToString().ToUpper()}] {nameof(Rule)}: {Rule.Id}, {nameof(PluginId)}: {PluginId}, " +
            $"{nameof(Message)}: {Message}\n\t{Location}";
    }
}