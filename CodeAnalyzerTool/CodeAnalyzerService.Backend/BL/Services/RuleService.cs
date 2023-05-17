using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using NuGet.Packaging;

namespace CodeAnalyzerService.Backend.BL.Services;

public class RuleService
{
    private readonly CodeAnalyzerServiceDbContext _ctx;
    
    public RuleService(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
    }

    public Rule GetRuleModelFromDto(RuleDto ruleDto)
    {
        var rule = GetRule(ruleDto.RuleName);
        if (rule is not null) return rule;
        rule = RuleMapper.MapToModel(ruleDto);
        rule = _ctx.Rules.Add(rule).Entity;
        _ctx.SaveChanges();

        return rule;
    }
    
    public Rule? GetRule(string ruleName)
    {
        return _ctx.Rules.SingleOrDefault(r => r.RuleName.Equals(ruleName));
    }
    
    public void AddRuleViolationsToRule(ProjectAnalysisDto projectAnalysisDto, Rule r)
    {
        r.RuleViolations.AddRange(
            projectAnalysisDto.RuleViolations
                .Where(rv => rv.Rule.RuleName.Equals(r.RuleName))
                .Select(RuleViolationMapper.MapToModel));
        _ctx.Rules.Update(r);
        _ctx.SaveChanges();
    }
}