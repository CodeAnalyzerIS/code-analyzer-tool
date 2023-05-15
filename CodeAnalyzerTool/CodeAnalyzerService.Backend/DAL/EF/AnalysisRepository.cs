using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class AnalysisRepository : IAnalysisRepository
{
    private readonly CasDbContext _context;

    public AnalysisRepository()
    {
        _context = new CasDbContext();
    }
    
    public AnalysisResult? ReadAnalysisResult(int id)
    {
        return _context.AnalysisResults.Find(id);
    }

    public AnalysisResult? ReadAnalysisResultWithRule(int id)
    {
        return _context.AnalysisResults.Include(ar => ar.Rule)
            .Single(ar => ar.Id == id);
    }

    public IEnumerable<AnalysisResult> ReadAnalysisResultsFromProject(int projectId)
    {
        throw new NotImplementedException();
    }

    public void CreateAnalysisResult(AnalysisResult analysisResult)
    {
        
    }

    public void DeleteAnalysisResult(int id)
    {
        throw new NotImplementedException();
    }

    public AnalysisResult UpdateAnalysisResult(AnalysisResult analysisResult)
    {
        throw new NotImplementedException();
    }
}