using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Sender.DTO;

public class ProjectAnalysisDto
{
    [Required] public string ProjectName { get; set; }
    public string? RepoUrl { get; set; }
    [Required] public IEnumerable<RuleViolationDto> RuleViolations { get; set; }

    public ProjectAnalysisDto(string projectName, IEnumerable<RuleViolationDto> ruleViolations, string? repoUrl)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
        RepoUrl = repoUrl;
    }
}