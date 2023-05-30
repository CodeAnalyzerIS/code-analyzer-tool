
namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectDetailsResponse
{
    public int Id { get; set; }
    public string ProjectName { get; set; } = null!;
    public AnalysisResponse LastAnalysis { get; set; } = null!;
    public IEnumerable<AnalysisWithViolationCountResponse> AnalysisHistory { get; set; }

    public ProjectDetailsResponse()
    {
        AnalysisHistory = new List<AnalysisWithViolationCountResponse>();
    }

    public ProjectDetailsResponse(int id, string projectName,
        IEnumerable<AnalysisWithViolationCountResponse> analysisHistory, AnalysisResponse analysis)
    {
        Id = id;
        ProjectName = projectName;
        AnalysisHistory = analysisHistory;
        LastAnalysis = analysis;
    }
}