using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class RuleViolation
{
    public int Id { get; set; }
    [Required] public Rule Rule { get; set; } = null!;
    [Required] public string Message { get; set; } = null!;
    [Required] public Location Location { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public Analysis Analysis { get; set; } = null!;

    // ReSharper disable once UnusedMember.Local => Used by Entity Framework
    private RuleViolation()
    {
    }

    public RuleViolation(string message, Location location, string severity)
    {
        Message = message;
        Location = location;
        Severity = severity;
    }

    public RuleViolation(Rule rule, string message, Location location, string severity, Analysis analysis) : this(
        message, location, severity)
    {
        Rule = rule;
        Analysis = analysis;
    }

    public override string ToString()
    {
        return
            $"[{Severity.ToUpper()}] {nameof(Rule)}: {Rule.RuleName}, {nameof(Rule.PluginName)}: {Rule.PluginName}, " +
            $"{nameof(Message)}: {Message}\n\t{Location}";
    }
}