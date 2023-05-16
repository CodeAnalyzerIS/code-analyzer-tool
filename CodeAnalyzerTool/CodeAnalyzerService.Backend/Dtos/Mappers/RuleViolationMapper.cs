using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class RuleViolationMapper
{
    public static RuleViolationDto MapToDto(RuleViolation ruleViolation)
    {
        var ruleDto = RuleMapper.MapToDto(ruleViolation.Rule);
        var locationDto = LocationMapper.MapToDto(ruleViolation.Location);
        
        return new RuleViolationDto(ruleDto, ruleViolation.PluginId, ruleViolation.Message,
            ruleViolation.TargetLanguage, locationDto, ruleViolation.Severity);
    }

    public static RuleViolation MapToModel(RuleViolationDto ruleViolationDto)
    {
        var rule = RuleMapper.MapToModel(ruleViolationDto.Rule);
        var location = LocationMapper.MapToModel(ruleViolationDto.Location);

        return new RuleViolation(rule, ruleViolationDto.PluginId, ruleViolationDto.Message,
            ruleViolationDto.TargetLanguage, location, ruleViolationDto.Severity);
    }
}