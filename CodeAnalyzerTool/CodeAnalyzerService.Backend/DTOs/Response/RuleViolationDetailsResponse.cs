namespace CodeAnalyzerService.Backend.DTOs.Response;


public class RuleViolationDetailsResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = null!;
    public LocationResponse Location { get; set; } = null!;
    public string Severity { get; set; } = null!;
    public DateTime AnalysisDate { get; set; }
    public RuleResponse Rule { get; set; } = null!;
}