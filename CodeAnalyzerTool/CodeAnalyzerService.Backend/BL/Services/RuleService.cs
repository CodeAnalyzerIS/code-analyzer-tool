using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using CodeAnalyzerService.Backend.DTOs.Request;

namespace CodeAnalyzerService.Backend.BL.Services;

public class RuleService
{
    private readonly CodeAnalyzerServiceDbContext _ctx;
    
    public RuleService(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
    }

    public Rule GetRuleModelFromDto(RuleRequest ruleRequest)
    {
        var rule = GetRule(ruleRequest.RuleName);
        if (rule is not null) return rule;
        rule = RuleMapper.MapToModel(ruleRequest);
        rule = _ctx.Rules.Add(rule).Entity;
        _ctx.SaveChanges();

        return rule;
    }
    
    public Rule? GetRule(string ruleName)
    {
        return _ctx.Rules.SingleOrDefault(r => r.RuleName.Equals(ruleName));
    }
}