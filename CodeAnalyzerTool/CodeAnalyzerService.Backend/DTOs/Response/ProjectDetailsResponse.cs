
namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectDetailsResponse
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public int LastAnalysisId { get; set; }
    public IEnumerable<AnalysisWithViolationCountResponse> AnalysisHistory { get; set; }

    public ProjectDetailsResponse()
    {
        AnalysisHistory = new List<AnalysisWithViolationCountResponse>();
    }

    public ProjectDetailsResponse(int id, string projectName,
        IEnumerable<AnalysisWithViolationCountResponse> analysisHistory, int analysisId)
    {
        Id = id;
        ProjectName = projectName;
        AnalysisHistory = analysisHistory;
        LastAnalysisId = analysisId;
    }
}