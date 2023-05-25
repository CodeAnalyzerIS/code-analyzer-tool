using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.DTOs.Request;

public class ProjectAnalysisRequest
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<RuleViolationRequest> RuleViolations { get; set; }

    public ProjectAnalysisRequest(string projectName, IEnumerable<RuleViolationRequest> ruleViolations)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
    }
}