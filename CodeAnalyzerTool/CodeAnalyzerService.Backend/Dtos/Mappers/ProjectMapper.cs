using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class ProjectMapper
{
    public static Project MapProjectAnalysisDtoToProject(ProjectAnalysisDto projectAnalysisDto)
    {
        return new Project(projectAnalysisDto.ProjectName);
    }

    public static ProjectDto MapToDto(Project project)
    {
        var analyses = project.Analyses.Select(AnalysisMapper.MapToDto);
        return new ProjectDto(project.ProjectName, analyses);
    }
}