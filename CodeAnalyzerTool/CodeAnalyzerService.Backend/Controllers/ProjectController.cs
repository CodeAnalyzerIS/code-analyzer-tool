using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeAnalyzerService.Backend.BL.Managers;
using CodeAnalyzerService.Backend.BL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;
using NuGet.Packaging;

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

        // GET: api/ProjectAnalysis
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            return await _context.Projects.ToListAsync();
        }
        
        [HttpGet("overview")]
        public ActionResult<IEnumerable<ProjectOverviewResponse>> GetProjectsOverview()
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

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

        // GET: api/ProjectAnalysis/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectResponse>> GetProject(int id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(p => p.Analyses)
                .ThenInclude(a => a.RuleViolations)
                .ThenInclude(rv => rv.Location)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var projectDto = ProjectMapper.MapToDto(project);

            return projectDto;
        }

        // PUT: api/ProjectAnalysis
        [HttpPut]
        public async Task<ActionResult<ProjectResponse>> PutProject(ProjectAnalysisRequest projectAnalysisRequest)
        {
            var project = await _projectAnalysisManager.AddProjectAnalysis(projectAnalysisRequest);

            var projectDto = ProjectMapper.MapToDto(project);

            return CreatedAtAction("GetProject", new { id = project.Id }, projectDto);
        }

        // DELETE: api/ProjectAnalysis/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProjectExists(int id)
        {
            return _context.Projects.Any(e => e.Id == id);
        }

        private Project? GetProject(string projectName)
        {
            return _context.Projects.SingleOrDefault(p => p.ProjectName.Equals(projectName));
        }
    }
}