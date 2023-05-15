using CAS_Backend.DAL.EF.Entities;

namespace CAS_Backend.DAL;

public interface IAnalysisRepository
{
    AnalysisResult? ReadAnalysisResult(int id);
    AnalysisResult? ReadAnalysisResultWithRule(int id);
    IEnumerable<AnalysisResult> ReadAnalysisResultsFromProject(int projectId);
    void CreateAnalysisResult(AnalysisResult analysisResult);
    void DeleteAnalysisResult(int id);
    AnalysisResult UpdateAnalysisResult(AnalysisResult analysisResult);
}