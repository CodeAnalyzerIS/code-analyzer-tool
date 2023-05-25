using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.API;

public class RuleViolation
{
    [Required] public Rule Rule { get; set; }
    [Required] public string Message { get; set; }
    [Required] public Location Location { get; set; }
    public Severity Severity { get; set; }

    public RuleViolation(Rule rule, string message, Location location, Severity severity)
    {
        Rule = rule;
        Message = message;
        Location = location;
        Severity = severity;
    }

    public override string ToString()
    {
        return
            $"[{Severity.ToString().ToUpper()}] {nameof(Rule)}: {Rule.RuleName}, {nameof(Rule.PluginName)}: {Rule.PluginName}, " +
            $"{nameof(Message)}: {Message}\n\t{Location}";
    }
}