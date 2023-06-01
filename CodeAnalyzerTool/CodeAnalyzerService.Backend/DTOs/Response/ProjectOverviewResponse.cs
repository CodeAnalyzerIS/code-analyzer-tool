namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectOverviewResponse
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public DateTime LastAnalysisDate { get; set; }
    public int RuleViolationCount { get; set; }

    public ProjectOverviewResponse()
    {
    }
    
    public ProjectOverviewResponse(int id, string projectName, DateTime lastAnalysisDate, int ruleViolationCount)
    {
        Id = id;
        ProjectName = projectName;
        LastAnalysisDate = lastAnalysisDate;
        RuleViolationCount = ruleViolationCount;
    }
}