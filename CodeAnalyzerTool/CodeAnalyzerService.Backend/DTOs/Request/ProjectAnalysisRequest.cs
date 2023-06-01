using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.DTOs.Request;

public class ProjectAnalysisRequest
{
    [Required] public string ProjectName { get; set; }
    public string? RepoUrl { get; set; }
    [Required] public IEnumerable<RuleViolationRequest> RuleViolations { get; set; }

    public ProjectAnalysisRequest(string projectName, string? repoUrl, IEnumerable<RuleViolationRequest> ruleViolations)
    {
        ProjectName = projectName;
        RepoUrl = repoUrl;
        RuleViolations = ruleViolations;
    }
}