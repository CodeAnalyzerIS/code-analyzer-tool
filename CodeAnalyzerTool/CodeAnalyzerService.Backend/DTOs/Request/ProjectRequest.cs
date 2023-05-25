using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.DTOs.Request;

public class ProjectRequest
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<RuleViolationRequest> RuleViolations { get; set; }

    public ProjectRequest(string projectName, IEnumerable<RuleViolationRequest> ruleViolations)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
    }
}