using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class AnalysisMapper
{
    public static AnalysisResponse MapToDto(Analysis analysis)
    {
        var ruleViolations = analysis.RuleViolations.Select(RuleViolationMapper.MapToDto);
        return new AnalysisResponse(analysis.Id, analysis.CreatedOn, ruleViolations);
    }
}