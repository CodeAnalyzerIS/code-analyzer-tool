namespace CodeAnalyzerService.Backend.Dtos;

public class ProjectDto
{
    public string ProjectName { get; set; }
    public IEnumerable<AnalysisDto> Analyses { get; set; }

    public ProjectDto(string projectName)
    {
        ProjectName = projectName;
        Analyses = new List<AnalysisDto>();
    }
    
    public ProjectDto(string projectName, IEnumerable<AnalysisDto> analyses)
    {
        ProjectName = projectName;
        Analyses = analyses;
    }
}