using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Sender.DTO;

public class ProjectAnalysisDto
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<RuleViolationDto> RuleViolations { get; set; }

    public ProjectAnalysisDto(string projectName, IEnumerable<RuleViolationDto> ruleViolations)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
    }
}