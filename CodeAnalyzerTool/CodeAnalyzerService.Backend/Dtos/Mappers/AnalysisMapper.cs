using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class AnalysisMapper
{
    public static AnalysisDto MapToDto(Analysis analysis)
    {
        var ruleViolations = analysis.RuleViolations.Select(RuleViolationMapper.MapToDto);
        return new AnalysisDto(analysis.CreatedOn, ruleViolations);
    }
}