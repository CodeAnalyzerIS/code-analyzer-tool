
namespace CodeAnalyzerService.Backend.DTOs.Response;

public class AnalysisResponse
{
    public int Id { get; set; }
    public string CreatedOn { get; set; }
    public IEnumerable<RuleViolationResponse> RuleViolations { get; set; }

    public AnalysisResponse(int id, string createdOn)
    {
        CreatedOn = createdOn;
        RuleViolations = new List<RuleViolationResponse>();
    }
    
    public AnalysisResponse(int id, string createdOn, IEnumerable<RuleViolationResponse> ruleViolations)
    {
        Id = id;
        CreatedOn = createdOn;
        RuleViolations = ruleViolations;
    }
}