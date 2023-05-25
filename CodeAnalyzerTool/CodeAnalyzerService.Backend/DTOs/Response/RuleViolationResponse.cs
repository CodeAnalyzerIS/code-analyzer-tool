using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DTOs.Response;

public class RuleViolationResponse
{
    public int Id { get; set; }
    [Required] public RuleResponse Rule { get; set; }
    [Required] public string Message { get; set; }
    [Required] public LocationResponse Location { get; set; }
    public string Severity { get; set; }
    
    public RuleViolationResponse(int id, RuleResponse rule, string message, LocationResponse location, string severity)
    {
        Id = id;
        Rule = rule;
        Message = message;
        Location = location;
        Severity = severity;
    }
}