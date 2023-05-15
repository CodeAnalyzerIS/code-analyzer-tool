using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.DAL;

public interface IProjectAnalysisRepository
{
    Project ReadProjectAnalysisWithRuleViolations(int projectId);
    void CreateProjectAnalysis(Project project);
    void DeleteProjectAnalysis(int projectId);
}