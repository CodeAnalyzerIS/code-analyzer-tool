using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Project
{
    public int Id { get; set; }
    [Required] public string ProjectName { get; set; } = null!;
    [Required] public ICollection<Analysis> Analyses { get; set; } = null!;

    private Project()
    {
    }

    public Project(string projectName)
    {
        ProjectName = projectName;
        Analyses = new List<Analysis>();
    }

    public Project(string projectName, ICollection<Analysis> analyses)
    {
        ProjectName = projectName;
        Analyses = analyses;
    }
}