using CodeAnalyzerService.Backend.BL.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        private readonly CodeAnalyzerServiceDbContext _context;
        private readonly AddProjectAnalysisManager _projectAnalysisManager;

        public ProjectController(CodeAnalyzerServiceDbContext context)
        {
            _context = context;
            _projectAnalysisManager = new AddProjectAnalysisManager(_context);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            return await _context.Projects.ToListAsync();
        }
        
        [HttpGet("Overview")]
        public ActionResult<IEnumerable<ProjectOverviewResponse>> GetProjectsOverview()
        {
           var projectOverviewResponses = _context.Projects.Include(p => p.Analyses)
                .Select(p => new ProjectOverviewResponse
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    LastAnalysisDate = p.Analyses.OrderBy(a => a.CreatedOn).Last().CreatedOn.ToString("dd-MMM-yyyy, HH:mm:ss"),
                    RuleViolationCount = p.Analyses.OrderBy(a => a.CreatedOn).Last().RuleViolations.Count()
                })
                .ToList();

            return projectOverviewResponses;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDetailsResponse>> GetProject(int id)
        {
            var project = await _context.Projects.Include(p => p.Analyses)
                .ThenInclude(a => a.RuleViolations)
                .ThenInclude(rv => rv.Rule)
                .Include(p => p.Analyses)
                .ThenInclude(a => a.RuleViolations)
                .ThenInclude(rv => rv.Location)
                .Where(p => p.Id == id)
                .Select(p => new ProjectDetailsResponse
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    LastAnalysis = AnalysisMapper.MapToDto(p.Analyses.OrderBy(a => a.CreatedOn).Last()),
                    AnalysisHistory = p.Analyses.Select(a => new AnalysisWithViolationCountResponse
                    {
                        Id = a.Id, 
                        CreatedOn = a.CreatedOn.ToString("dd-MMM-yyyy, HH:mm:ss"),
                        RuleViolationCount = a.RuleViolations.Count()
                    })
                }).SingleOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            return project;
        }

        [HttpPut]
        public async Task<ActionResult<ProjectResponse>> PutProject(ProjectAnalysisRequest projectAnalysisRequest)
        {
            var project = await _projectAnalysisManager.AddProjectAnalysis(projectAnalysisRequest);

            var projectDto = ProjectMapper.MapToDto(project);

            return CreatedAtAction("GetProject", new { id = project.Id }, projectDto);
        }
    }
}