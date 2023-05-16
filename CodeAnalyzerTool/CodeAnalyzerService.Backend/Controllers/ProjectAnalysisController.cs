using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.Dtos;

namespace CodeAnalyzerService.Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAnalysisController : ControllerBase
    {
        private readonly CodeAnalyzerServiceDbContext _context;

        public ProjectAnalysisController(CodeAnalyzerServiceDbContext context)
        {
            _context = context;
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
        public async Task<ActionResult<Project>> GetProject(int id)
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

            return project;
        }

        // PUT: api/ProjectAnalysis/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProject(int id, Project project)
        {
            if (id != project.Id)
            {
                return BadRequest();
            }

            _context.Entry(project).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProjectExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ProjectAnalysis
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Project>> PostProject(ProjectAnalysisDto projectAnalysisDto)
        {
            if (_context.Projects == null)
            {
                return Problem("Entity set 'CodeAnalyzerServiceDbContext.Projects'  is null.");
            }

            var project = new Project(projectAnalysisDto.ProjectName);
            var analysis = new Analysis(DateTime.Now, project);

            ICollection<RuleViolation> ruleViolations = (from ruleViolation in projectAnalysisDto.RuleViolations
                let location =
                    new Location(ruleViolation.Location.Path, ruleViolation.Location.StartLine,
                        ruleViolation.Location.EndLine, ruleViolation.Location.FileExtension)
                let rule = new Rule(ruleViolation.Rule.RuleName, ruleViolation.Rule.Title,
                    ruleViolation.Rule.Description, ruleViolation.Rule.Category, ruleViolation.Rule.IsEnabledByDefault,
                    ruleViolation.Rule.DefaultSeverity)
                select new RuleViolation(rule, ruleViolation.PluginId, ruleViolation.Message,
                    ruleViolation.TargetLanguage, location, ruleViolation.Severity, analysis)).ToList();
            analysis.RuleViolations = ruleViolations;
            project.Analyses.Add(analysis);

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProject", new { id = project.Id }, project);
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
            return (_context.Projects?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}