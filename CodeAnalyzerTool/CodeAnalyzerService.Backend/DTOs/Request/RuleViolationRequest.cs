using System.ComponentModel.DataAnnotations;
using CodeAnalyzerService.Backend.Dtos;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.DTOs.Request;

public class RuleViolationRequest
{
    [Required] public RuleRequest Rule { get; set; }
    [Required] public string Message { get; set; }
    [Required] public LocationRequest Location { get; set; }
    public string Severity { get; set; }
    
    public RuleViolationRequest(RuleRequest rule, string message, LocationRequest location, string severity)
    {
        Rule = rule;
        Message = message;
        Location = location;
        Severity = severity;
    }
}