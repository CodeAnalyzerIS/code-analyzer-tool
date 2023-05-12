using System.ComponentModel.DataAnnotations;

namespace CAT_API;

public class ProjectAnalysis
{
    [Required] public string ProjectId { get; set; }
    [Required] public IEnumerable<AnalysisResult> AnalysisResults { get; set; }

    public ProjectAnalysis(string projectId, IEnumerable<AnalysisResult> analysisResults)
    {
        ProjectId = projectId;
        AnalysisResults = analysisResults;
    }
}