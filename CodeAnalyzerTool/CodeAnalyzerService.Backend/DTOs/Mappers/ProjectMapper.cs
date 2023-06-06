using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public static class ProjectMapper
{
    public static Project MapToModel(this ProjectAnalysisRequest projectAnalysisRequest)
    {
        return new Project(projectAnalysisRequest.ProjectName, projectAnalysisRequest.RepoUrl);
    }

    public static ProjectResponse MapToDto(this Project project)
    {
        var analyses = project.Analyses.Select(AnalysisMapper.MapToDto);
        return new ProjectResponse(project.Id, project.ProjectName, analyses);
    }
}