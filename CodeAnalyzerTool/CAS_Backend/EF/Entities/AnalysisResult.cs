using System.ComponentModel.DataAnnotations;
using CAT_API;

namespace CAS_Backend.EF.Entities;

public class AnalysisResult
{
    public int Id { get; set; }
    [Required] public Rule Rule { get; set; } = null!;
    [Required] public string PluginId { get; set; } = null!;
    [Required] public string Message { get; set; } = null!;
    [Required] public string TargetLanguage { get; set; } = null!;
    [Required] public Location Location { get; set; } = null!;
    public Severity Severity { get; set; }

    private AnalysisResult()
    {
    }

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
            $"[{Severity.ToString().ToUpper()}] {nameof(Rule)}: {Rule.RuleName}, {nameof(PluginId)}: {PluginId}, " +
            $"{nameof(Message)}: {Message}\n\t{Location}";
    }
}