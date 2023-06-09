namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Analysis
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    public IEnumerable<RuleViolation> RuleViolations { get; set; }
    public Project Project { get; set; } = null!;
    
    // ReSharper disable once UnusedMember.Local => Used by Entity Framework
    private Analysis()
    {
        RuleViolations = new List<RuleViolation>();
    }
    
    public Analysis(DateTime createdOn)
    {
        CreatedOn = createdOn;
        RuleViolations = new List<RuleViolation>();
    }

    public Analysis(DateTime createdOn, Project project)
    {
        CreatedOn = createdOn;
        Project = project;
        RuleViolations = new List<RuleViolation>();
    }
    
    public Analysis(DateTime createdOn, Project project, IEnumerable<RuleViolation> ruleViolations)
    {
        CreatedOn = createdOn;
        Project = project;
        RuleViolations = ruleViolations;
    }
}