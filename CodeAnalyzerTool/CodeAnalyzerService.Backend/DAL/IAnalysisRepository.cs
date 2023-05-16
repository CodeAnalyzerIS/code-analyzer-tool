using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.DAL;

public interface IAnalysisRepository
{
    Analysis? ReadAnalysis(int id);
    Analysis ReadAnalysisWithRuleViolations(int id);
    void CreateAnalysis(Analysis analysis);
    void DeleteAnalysis(int id);
}