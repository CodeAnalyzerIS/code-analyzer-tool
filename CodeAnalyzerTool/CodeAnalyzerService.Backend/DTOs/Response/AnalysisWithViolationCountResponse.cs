namespace CodeAnalyzerService.Backend.DTOs.Response;

public class AnalysisWithViolationCountResponse
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public int RuleViolationCount { get; set; }

    public AnalysisWithViolationCountResponse()
    {
    }

    public AnalysisWithViolationCountResponse(int id, DateTime createdOn, int ruleViolationCount)
    {
        Id = id;
        CreatedOn = createdOn;
        RuleViolationCount = ruleViolationCount;
    }
}