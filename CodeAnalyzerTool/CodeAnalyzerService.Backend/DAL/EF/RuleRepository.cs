using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.DAL.EF;

public class RuleRepository : IRuleRepository
{
    private readonly CodeAnalyzerServiceDbContext _ctx;

    public RuleRepository(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
    }
    
    public Rule? ReadRule(int id)
    {
        return _ctx.Rules.Find(id);
    }

    public void CreateRule(Rule rule)
    {
        throw new NotImplementedException();
    }

    public void DeleteRule(int id)
    {
        throw new NotImplementedException();
    }
}