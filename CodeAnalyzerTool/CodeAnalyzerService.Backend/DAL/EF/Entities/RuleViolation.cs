using System.ComponentModel.DataAnnotations;
using CodeAnalyzerTool.Api;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class RuleViolation
{
    public int Id { get; set; }
    [Required] public Rule Rule { get; set; } = null!;
    [Required] public string PluginId { get; set; } = null!;
    [Required] public string Message { get; set; } = null!;
    [Required] public string TargetLanguage { get; set; } = null!;
    [Required] public Location Location { get; set; } = null!;
    public Severity Severity { get; set; }
    public Analysis Analysis { get; set; } = null!;

    private RuleViolation()
    {
    }

    public RuleViolation(Rule rule, string pluginId, string message, string targetLanguage, Location location,
        Severity severity, Analysis analysis)
    {
        Rule = rule;
        PluginId = pluginId;
        Message = message;
        TargetLanguage = targetLanguage;
        Location = location;
        Severity = severity;
        Analysis = analysis;
    }

    public override string ToString()
    {
        return
            $"[{Severity.ToString().ToUpper()}] {nameof(Rule)}: {Rule.RuleName}, {nameof(PluginId)}: {PluginId}, " +
            $"{nameof(Message)}: {Message}\n\t{Location}";
    }
}