using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.Dtos;

public class ProjectDto
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<RuleViolationDto> RuleViolations { get; set; }

    public ProjectDto(string projectName, IEnumerable<RuleViolationDto> ruleViolations)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
    }
}