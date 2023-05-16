using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class ProjectMapper
{
    public static Project MapProjectAnalysisDtoToProject(ProjectAnalysisDto projectAnalysisDto)
    {
        var analysis = new Analysis(DateTime.Now);
        var ruleViolations = projectAnalysisDto.RuleViolations.Select(RuleViolationMapper.MapToModel);
        analysis.RuleViolations = ruleViolations;
        var project = new Project(projectAnalysisDto.ProjectName);
        project.Analyses.Add(analysis);

        return project;
    }

    public static ProjectDto MapToDto(Project project)
    {
        var analyses = project.Analyses.Select(AnalysisMapper.MapToDto);
        return new ProjectDto(project.ProjectName, analyses);
    }
}