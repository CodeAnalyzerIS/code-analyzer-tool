using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class RuleViolationRepository : IRuleViolationRepository
{
    private readonly CodeAnalyzerServiceDbContext _context;

    public RuleViolationRepository()
    {
        _context = new CodeAnalyzerServiceDbContext();
    }
    
    public RuleViolation? ReadRuleViolation(int id)
    {
        return _context.AnalysisResults.Find(id);
    }

    public RuleViolation ReadRuleViolationWithRule(int id)
    {
        return _context.AnalysisResults.Include(ar => ar.Rule)
            .Single(ar => ar.Id == id);
    }

    public IEnumerable<RuleViolation> ReadAnalysisResultsFromProject(int projectId)
    {
        throw new NotImplementedException();
    }

    public void CreateRuleViolation(RuleViolation ruleViolation)
    {
        throw new NotImplementedException();
    }

    public void DeleteRuleViolation(int id)
    {
        throw new NotImplementedException();
    }
}