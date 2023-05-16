using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class ProjectRepository : IProjectRepository
{
    private readonly CodeAnalyzerServiceDbContext _ctx;

    public ProjectRepository(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
    }
    
    public Project ReadProjectWithAnalyses(int projectId)
    {
        return _ctx.Projects.Include(p => p.Analyses)
            .Single(p => p.Id == projectId);
    }

    public void CreateProject(Project project)
    {
        throw new NotImplementedException();
    }

    public void DeleteProject(int projectId)
    {
        throw new NotImplementedException();
    }
}