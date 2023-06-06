using CodeAnalyzerService.Backend.BL.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAnalyzerService.Backend.DAL.EF;
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

        [HttpGet("GetFromName/{projectName}")]
        public async Task<ActionResult<int>> GetProjectIdFromProjectName(string projectName)
        {
            var project =
                await _context.Projects.SingleOrDefaultAsync(p =>
                    p.ProjectName.ToLower().Equals(projectName.ToLower()));
            if (project == null)
            {
                return NotFound();
            }

            return project.Id;
        }

        [HttpGet("Overview")]
        public ActionResult<IEnumerable<ProjectOverviewResponse>> GetProjectsOverview()
        {
            var projectOverviewResponses = _context.Projects.Include(p => p.Analyses)
                .Select(p => new ProjectOverviewResponse
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    LastAnalysisDate = p.Analyses.OrderBy(a => a.CreatedOn).Last().CreatedOn.ToUniversalTime(),
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
                .Where(p => p.Id == id)
                .Select(p => new ProjectDetailsResponse
                {
                    Id = p.Id,
                    ProjectName = p.ProjectName,
                    RepoUrl = p.RepoUrl,
                    LastAnalysisId = p.Analyses.OrderBy(a => a.CreatedOn).Last().Id,
                    RuleViolationCount = p.Analyses.OrderBy(a => a.CreatedOn).Last().RuleViolations.Count(),
                    LastAnalysisDate = p.Analyses.OrderBy(a => a.CreatedOn).Last().CreatedOn.ToUniversalTime(),
                    AnalysisHistory = p.Analyses.OrderByDescending(a => a.CreatedOn)
                        .Select(a => new AnalysisHistoryResponse
                        {
                            Id = a.Id,
                            CreatedOn = a.CreatedOn.ToUniversalTime(),
                        }),
                    RuleViolationHistory = p.Analyses.Select(a => a.RuleViolations.Count()).ToArray(),
                    RuleViolationDifference =
                        p.Analyses.OrderByDescending(a => a.CreatedOn).Skip(1).First().RuleViolations.Count() -
                        p.Analyses.OrderBy(a => a.CreatedOn).Last().RuleViolations.Count(),
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