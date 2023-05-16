namespace CodeAnalyzerService.Backend.Dtos;

public class AnalysisDto
{
    public DateTime CreatedOn { get; set; }
    public IEnumerable<RuleViolationDto> RuleViolations { get; set; }

    public AnalysisDto(DateTime createdOn)
    {
        CreatedOn = createdOn;
        RuleViolations = new List<RuleViolationDto>();
    }
    
    public AnalysisDto(DateTime createdOn, IEnumerable<RuleViolationDto> ruleViolations)
    {
        CreatedOn = createdOn;
        RuleViolations = ruleViolations;
    }
}