using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.DAL;

public interface IRuleViolationRepository
{
    RuleViolation? ReadRuleViolation(int id);
    RuleViolation ReadRuleViolationWithRule(int id);
    void CreateRuleViolation(RuleViolation ruleViolation);
    void DeleteRuleViolation(int id);
}