using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.DAL;

public interface IRuleRepository
{
    Rule? ReadRule(int id);
    void CreateRule(Rule rule);
    void DeleteRule(int id);
}