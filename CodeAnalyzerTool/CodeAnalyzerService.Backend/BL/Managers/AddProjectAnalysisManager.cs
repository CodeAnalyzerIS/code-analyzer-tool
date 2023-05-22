using CodeAnalyzerService.Backend.BL.Services;
using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

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

    public async Task<Project> AddProjectAnalysis(ProjectAnalysisDto projectAnalysisDto)
    {
        var rules = GetRulesFromProjectAnalysisDto(projectAnalysisDto);

        var ruleViolations = GetRuleViolationsAndAssignRule(projectAnalysisDto, rules);

        var analysis = CreateAnalysisAndAssignRuleViolations(ruleViolations);
        
        var project = GetProject(projectAnalysisDto.ProjectName);

        project = AddOrUpdateProject(projectAnalysisDto, project, analysis);
        
        await _ctx.SaveChangesAsync();

        return project;
    }

    private IEnumerable<Rule> GetRulesFromProjectAnalysisDto(ProjectAnalysisDto projectAnalysisDto)
    {
        return projectAnalysisDto.RuleViolations
            .Select(rv => rv.Rule)
            .Select(_ruleService.GetRuleModelFromDto)
            .Distinct();
    }

    private static IEnumerable<RuleViolation> GetRuleViolationsAndAssignRule(ProjectAnalysisDto projectAnalysisDto, IEnumerable<Rule> rules)
    {
        return projectAnalysisDto.RuleViolations.Select(rv =>
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

    private Project AddOrUpdateProject(ProjectAnalysisDto projectAnalysisDto, Project? project, Analysis analysis)
    {
        if (project is null)
        {
            project = ProjectMapper.MapProjectAnalysisDtoToProject(projectAnalysisDto);
            project.Analyses.Add(analysis);
            project = _ctx.Projects.Add(project).Entity;
        }
        else
        {
            project.Analyses.Add(analysis);
            _ctx.Entry(project).State = EntityState.Modified;
        }

        return project;
    }
}