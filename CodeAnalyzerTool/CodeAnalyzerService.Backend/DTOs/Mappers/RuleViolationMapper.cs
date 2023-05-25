using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public static class RuleViolationMapper
{
    public static RuleViolationResponse MapToDto(RuleViolation ruleViolation)
    {
        var ruleDto = RuleMapper.MapToDto(ruleViolation.Rule);
        var locationDto = LocationMapper.MapToDto(ruleViolation.Location);

        return new RuleViolationResponse(ruleViolation.Id, ruleDto, ruleViolation.Message, locationDto,
            ruleViolation.Severity);
    }

    public static RuleViolation MapToModel(RuleViolationRequest ruleViolationRequest)
    {
        var location = LocationMapper.MapToModel(ruleViolationRequest.Location);

        return new RuleViolation(ruleViolationRequest.Message, location, ruleViolationRequest.Severity);
    }
}