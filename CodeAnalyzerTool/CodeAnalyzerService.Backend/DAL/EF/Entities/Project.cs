using Microsoft.Build.Framework;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Project
{
    public int Id { get; set; }
    [Required] public string ProjectName { get; set; } = null!;
    public string? RepoUrl { get; set; }
    [Required] public ICollection<Analysis> Analyses { get; set; }

    private Project()
    {
        Analyses = new List<Analysis>();
    }

    public Project(string projectName)
    {
        ProjectName = projectName;
        Analyses = new List<Analysis>();
    }
    
    public Project(string projectName, string? repoUrl)
    {
        ProjectName = projectName;
        RepoUrl = repoUrl;
        Analyses = new List<Analysis>();
    }

    public Project(string projectName, string? repoUrl, ICollection<Analysis> analyses)
    {
        ProjectName = projectName;
        RepoUrl = repoUrl;
        Analyses = analyses;
    }
}