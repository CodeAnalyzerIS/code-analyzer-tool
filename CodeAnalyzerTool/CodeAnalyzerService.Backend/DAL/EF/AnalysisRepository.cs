using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class AnalysisRepository : IAnalysisRepository
{
    private readonly CodeAnalyzerServiceDbContext _ctx;

    public AnalysisRepository(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
    }
    
    public Analysis? ReadAnalysis(int id)
    {
        return _ctx.Analyses.Find(id);
    }

    public Analysis ReadAnalysisWithRuleViolations(int id)
    {
        return _ctx.Analyses.Include(a => a.RuleViolations)
            .Single(a => a.Id == id);
    }

    public void CreateAnalysis(Analysis analysis)
    {
        throw new NotImplementedException();
    }

    public void DeleteAnalysis(int id)
    {
        throw new NotImplementedException();
    }
}