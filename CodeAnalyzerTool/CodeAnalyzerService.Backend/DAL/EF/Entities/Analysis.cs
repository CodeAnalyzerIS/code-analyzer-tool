namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Analysis
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public IEnumerable<RuleViolation> RuleViolations { get; set; } = null!;
    public Project Project { get; set; } = null!;
}