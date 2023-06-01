using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.API;

public class ProjectAnalysis
{
    [Required] public string ProjectName { get; set; }
    public string? RepoUrl { get; set; }
    [Required] public IEnumerable<RuleViolation> RuleViolations { get; set; }

    public ProjectAnalysis(string projectName, IEnumerable<RuleViolation> ruleViolations)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
    }

    public ProjectAnalysis(string projectName, string? repoUrl, IEnumerable<RuleViolation> ruleViolations) 
        : this(projectName, ruleViolations)
    {
        RepoUrl = repoUrl;
    }
}