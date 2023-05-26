using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.API;

public class ProjectAnalysis
{
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<RuleViolation> RuleViolations { get; set; }

    public ProjectAnalysis(string projectName, IEnumerable<RuleViolation> ruleViolations)
    {
        ProjectName = projectName;
        RuleViolations = ruleViolations;
    }
}