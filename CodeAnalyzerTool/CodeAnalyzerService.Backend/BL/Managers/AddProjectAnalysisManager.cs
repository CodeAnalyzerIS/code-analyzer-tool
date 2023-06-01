using CodeAnalyzerService.Backend.BL.Services;
using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using CodeAnalyzerService.Backend.DTOs.Request;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.BL.Managers;

public class AddProjectAnalysisManager
{
    private readonly CodeAnalyzerServiceDbContext _ctx;
    private readonly RuleService _ruleService;

    public AddProjectAnalysisManager(CodeAnalyzerServiceDbContext context)
    {
        _ctx = context;
        _ruleService = new RuleService(_ctx);
    }
    
    public AddProjectAnalysisManager(CodeAnalyzerServiceDbContext context, RuleService ruleService)
    {
        _ctx = context;
        _ruleService = ruleService;
    }

    public async Task<Project> AddProjectAnalysis(ProjectAnalysisRequest projectAnalysisRequest)
    {
        var rules = GetRulesFromProjectAnalysisDto(projectAnalysisRequest);

        var ruleViolations = GetRuleViolationsAndAssignRule(projectAnalysisRequest, rules);

        var analysis = CreateAnalysisAndAssignRuleViolations(ruleViolations);
        
        var project = GetProject(projectAnalysisRequest.ProjectName);

        project = AddOrUpdateProject(projectAnalysisRequest, project, analysis);
        
        await _ctx.SaveChangesAsync();

        return project;
    }

    private IEnumerable<Rule> GetRulesFromProjectAnalysisDto(ProjectAnalysisRequest projectAnalysisRequest)
    {
        return projectAnalysisRequest.RuleViolations
            .Select(rv => rv.Rule)
            .Select(_ruleService.GetRuleModelFromDto)
            .Distinct();
    }

    private static IEnumerable<RuleViolation> GetRuleViolationsAndAssignRule(ProjectAnalysisRequest projectAnalysisRequest, IEnumerable<Rule> rules)
    {
        return projectAnalysisRequest.RuleViolations.Select(rv =>
        {
            var rule = rules.Single(r => r.RuleName.Equals(rv.Rule.RuleName));
            var ruleViolation = RuleViolationMapper.MapToModel(rv);
            ruleViolation.Rule = rule;
            return ruleViolation;
        });
    }

    private static Analysis CreateAnalysisAndAssignRuleViolations(IEnumerable<RuleViolation> ruleViolations)
    {
        return new Analysis(DateTime.Now)
        {
            RuleViolations = ruleViolations.ToList()
        };
    }

    private Project? GetProject(string projectName)
    {
        return _ctx.Projects.SingleOrDefault(p => p.ProjectName.Equals(projectName));
    }

    private Project AddOrUpdateProject(ProjectAnalysisRequest projectAnalysisRequest, Project? project, Analysis analysis)
    {
        if (project is null)
        {
            project = ProjectMapper.MapToModel(projectAnalysisRequest);
            project.Analyses.Add(analysis);
            project = _ctx.Projects.Add(project).Entity;
        }
        else
        {
            project.RepoUrl = projectAnalysisRequest.RepoUrl;
            project.Analyses.Add(analysis);
            _ctx.Entry(project).State = EntityState.Modified;
        }

        return project;
    }
}