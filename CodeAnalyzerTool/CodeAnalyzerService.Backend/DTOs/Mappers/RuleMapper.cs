using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public static class RuleMapper
{
    public static RuleResponse MapToDto(this Rule rule)
    {
        return new RuleResponse(rule.Id, rule.RuleName, rule.Title, rule.Description, rule.Category, rule.PluginName,
            rule.TargetLanguage, rule.IsEnabledByDefault, rule.DefaultSeverity, rule.CodeExample, rule.CodeExampleFix);
    }

    public static Rule MapToModel(this RuleRequest ruleRequest)
    {
        return new Rule(ruleRequest.RuleName, ruleRequest.Title, ruleRequest.Description, ruleRequest.Category, ruleRequest.PluginName,
            ruleRequest.TargetLanguage, ruleRequest.IsEnabledByDefault, ruleRequest.DefaultSeverity, ruleRequest.CodeExample, ruleRequest.CodeExampleFix);
    }
}