using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Project
{
    public int Id { get; set; }
    [Required] public string ProjectName { get; set; }
    [Required] public IEnumerable<Analysis> Analyses { get; set; }
    
    public Project(string projectName, IEnumerable<Analysis> analyses)
    {
        ProjectName = projectName;
        Analyses = analyses;
    }
}