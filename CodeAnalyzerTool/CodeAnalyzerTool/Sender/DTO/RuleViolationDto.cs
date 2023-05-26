using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Sender.DTO;

public class RuleViolationDto
{
    [Required] public RuleDto Rule { get; set; }
    [Required] public string Message { get; set; }
    [Required] public LocationDto Location { get; set; }
    public string Severity { get; set; }

    public RuleViolationDto(RuleDto rule, string message, LocationDto location, string severity)
    {
        Rule = rule;
        Message = message;
        Location = location;
        Severity = severity;
    }
}