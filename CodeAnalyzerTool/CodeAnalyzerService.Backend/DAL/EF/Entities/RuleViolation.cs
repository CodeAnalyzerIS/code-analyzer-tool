using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class RuleViolation
{
    public int Id { get; set; }
    [Required] public Rule Rule { get; set; } = null!;
    [Required] public string PluginId { get; set; } = null!;
    [Required] public string Message { get; set; } = null!;
    [Required] public string TargetLanguage { get; set; } = null!;
    [Required] public Location Location { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public Analysis Analysis { get; set; } = null!;

    private RuleViolation()
    {
    }
    
    public RuleViolation(string pluginId, string message, string targetLanguage, Location location,
        string severity)
    {
        PluginId = pluginId;
        Message = message;
        TargetLanguage = targetLanguage;
        Location = location;
        Severity = severity;
    }

    public RuleViolation(Rule rule, string pluginId, string message, string targetLanguage, Location location,
        string severity, Analysis analysis) : this(pluginId, message, targetLanguage, location, severity)
    {
        Rule = rule;
        Analysis = analysis;
    }

    public override string ToString()
    {
        return
            $"[{Severity.ToUpper()}] {nameof(Rule)}: {Rule.RuleName}, {nameof(PluginId)}: {PluginId}, " +
            $"{nameof(Message)}: {Message}\n\t{Location}";
    }
}