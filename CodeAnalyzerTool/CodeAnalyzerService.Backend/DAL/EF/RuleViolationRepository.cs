using CodeAnalyzerService.Backend.DAL.EF.Entities;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class RuleViolationRepository : IRuleViolationRepository
{
    private readonly CodeAnalyzerServiceDbContext _ctx;

    public RuleViolationRepository(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
    }
    
    public RuleViolation? ReadRuleViolation(int id)
    {
        return _ctx.RuleViolations.Find(id);
    }

    public RuleViolation ReadRuleViolationWithRule(int id)
    {
        return _ctx.RuleViolations.Include(ar => ar.Rule)
            .Single(ar => ar.Id == id);
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