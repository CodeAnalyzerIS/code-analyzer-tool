using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using CodeAnalyzerService.Backend.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AnalysisController : ControllerBase
{
    private readonly CodeAnalyzerServiceDbContext _context;

    public AnalysisController(CodeAnalyzerServiceDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AnalysisResponse>> GetAnalysis(int id)
    {
        var analysis = await _context.Analyses.Include(a => a.RuleViolations)
            .ThenInclude(rv => rv.Location)
            .Include(a => a.RuleViolations)
            .ThenInclude(rv => rv.Rule)
            .SingleOrDefaultAsync(a => a.Id == id);

        if (analysis == null)
        {
            return NotFound();
        }

        return AnalysisMapper.MapToDto(analysis);
    }
}