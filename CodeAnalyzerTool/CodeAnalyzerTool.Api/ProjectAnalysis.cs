using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Api;

public class ProjectAnalysis
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<RuleViolation> AnalysisResults { get; set; }

    public ProjectAnalysis(string projectName, IEnumerable<RuleViolation> analysisResults)
    {
        ProjectName = projectName;
        AnalysisResults = analysisResults;
    }
}