using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class RuleMapper
{
    public static RuleDto MapToDto(Rule rule)
    {
        return new RuleDto(rule.RuleName, rule.Title, rule.Description, rule.Category, rule.IsEnabledByDefault,
            rule.DefaultSeverity);
    }
    
    public static Rule MapToModel(RuleDto ruleDto)
    {
        return new Rule(ruleDto.RuleName, ruleDto.Title, ruleDto.Description, ruleDto.Category, ruleDto.IsEnabledByDefault,
            ruleDto.DefaultSeverity);
    }
}