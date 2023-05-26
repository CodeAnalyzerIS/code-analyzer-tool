namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectOverviewResponse
{
    //TODO aan iemand vragen hoe ik entity framework enkel dit kan laten ophalen en in response laten steken
    public int Id { get; set; }
    public string ProjectName { get; set; }
    public DateTime LastAnalysis { get; set; }
    public int RuleViolationCount { get; set; }

    public ProjectOverviewResponse(int id, string projectName, DateTime lastAnalysis, int ruleViolationCount)
    {
        Id = id;
        ProjectName = projectName;
        LastAnalysis = lastAnalysis;
        RuleViolationCount = ruleViolationCount;
    }
}