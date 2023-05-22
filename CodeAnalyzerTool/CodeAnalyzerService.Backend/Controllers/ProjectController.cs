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

        // GET: api/ProjectAnalysis/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProjectDto>> GetProject(int id)
        {
            if (_context.Projects == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.Include(p => p.Analyses)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            var projectDto = ProjectMapper.MapToDto(project);

            return projectDto;
        }

        // POST: api/ProjectAnalysis
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<ActionResult<ProjectDto>> PutProject(ProjectAnalysisDto projectAnalysisDto)
        {
            var project = await _projectAnalysisManager.AddProjectAnalysis(projectAnalysisDto);

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