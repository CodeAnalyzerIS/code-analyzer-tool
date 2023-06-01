
namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectDetailsResponse
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public string? RepoUrl { get; set; }
    public int LastAnalysisId { get; set; }
    public int RuleViolationCount { get; set; } 
    public DateTime LastAnalysisDate { get; set; }
    public IEnumerable<AnalysisHistoryResponse> AnalysisHistory { get; set; }
    public int[] RuleViolationHistory { get; set; } = null!;
    public int RuleViolationDifference { get; set; }

    public ProjectDetailsResponse()
    {
        AnalysisHistory = new List<AnalysisHistoryResponse>();
    }

    public ProjectDetailsResponse(int id, string projectName, IEnumerable<AnalysisHistoryResponse> analysisHistory, 
        int analysisId, int[] ruleViolationHistory, int ruleViolationCount, int ruleViolationDifference)
    {
        Id = id;
        ProjectName = projectName;
        AnalysisHistory = analysisHistory;
        LastAnalysisId = analysisId;
        RuleViolationHistory = ruleViolationHistory;
        RuleViolationCount = ruleViolationCount;
        RuleViolationDifference = ruleViolationDifference;
    }
}