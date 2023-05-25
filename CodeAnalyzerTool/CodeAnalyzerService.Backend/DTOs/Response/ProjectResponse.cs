namespace CodeAnalyzerService.Backend.DTOs.Response;

public class ProjectResponse
{
    public int? Id { get; set; }
    public string ProjectName { get; set; }
    public IEnumerable<AnalysisResponse> Analyses { get; set; }

    public ProjectResponse(int id, string projectName)
    {
        ProjectName = projectName;
        Analyses = new List<AnalysisResponse>();
    }
    
    public ProjectResponse(int id, string projectName, IEnumerable<AnalysisResponse> analyses)
    {
        Id = id;
        ProjectName = projectName;
        Analyses = analyses;
    }
}