namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectOverviewResponse
{
    //TODO aan iemand vragen hoe ik entity framework enkel dit kan laten ophalen en in response laten steken
    public int Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public string LastAnalysisDate { get; set; } = null!;
    public int RuleViolationCount { get; set; }

    public ProjectOverviewResponse()
    {
    }
    
    public ProjectOverviewResponse(int id, string projectName, string lastAnalysisDate, int ruleViolationCount)
    {
        Id = id;
        ProjectName = projectName;
        LastAnalysisDate = lastAnalysisDate;
        RuleViolationCount = ruleViolationCount;
    }
}