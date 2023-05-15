using System.ComponentModel.DataAnnotations;

namespace CAT_API;

public class ProjectAnalysis
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<AnalysisResult> AnalysisResults { get; set; }

    public ProjectAnalysis(string projectName, IEnumerable<AnalysisResult> analysisResults)
    {
        ProjectName = projectName;
        AnalysisResults = analysisResults;
    }
}