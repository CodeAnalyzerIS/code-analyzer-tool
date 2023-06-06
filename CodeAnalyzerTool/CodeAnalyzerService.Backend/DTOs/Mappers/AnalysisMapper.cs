using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public static class AnalysisMapper
{
    public static AnalysisResponse MapToDto(this Analysis analysis)
    {
        var ruleViolations = analysis.RuleViolations.Select(RuleViolationMapper.MapToDto);
        return new AnalysisResponse(analysis.Id, analysis.CreatedOn.ToString("dd-MMM-yyyy, HH:mm:ss"), ruleViolations);
    }
}