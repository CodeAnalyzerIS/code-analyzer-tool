namespace CodeAnalyzerService.Backend.DTOs.Response;

public class AnalysisWithViolationCountResponse
{
    public int Id { get; set; }
    public string CreatedOn { get; set; } = null!;
    public int RuleViolationCount { get; set; }

    public AnalysisWithViolationCountResponse()
    {
    }

    public AnalysisWithViolationCountResponse(int id, string createdOn, int ruleViolationCount)
    {
        Id = id;
        CreatedOn = createdOn;
        RuleViolationCount = ruleViolationCount;
    }
}