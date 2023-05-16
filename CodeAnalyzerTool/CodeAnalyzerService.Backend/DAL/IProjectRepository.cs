using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.DAL;

public interface IProjectRepository
{
    Project ReadProjectWithAnalyses(int projectId);
    void CreateProject(Project project);
    void DeleteProject(int projectId);
}