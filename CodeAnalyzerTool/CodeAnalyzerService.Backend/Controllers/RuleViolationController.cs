using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.Dtos.Mappers;
using CodeAnalyzerService.Backend.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RuleViolationController : ControllerBase
{
    private readonly CodeAnalyzerServiceDbContext _context;

    public RuleViolationController(CodeAnalyzerServiceDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RuleViolationDetailsResponse>> GetRuleViolation(int id)
    {
        var ruleViolation = await _context.RuleViolations.Where(r => r.Id == id)
            .Select(rv => new RuleViolationDetailsResponse
                {
                    Id = rv.Id,
                    Message = rv.Message,
                    Location = rv.Location.MapToDto(),
                    Severity = rv.Severity,
                    AnalysisDate = rv.Analysis.CreatedOn.ToUniversalTime(),
                    Rule = rv.Rule.MapToDto()
                }
            )
            .AsNoTracking()
            .SingleOrDefaultAsync();
        
        if (ruleViolation == null) return NotFound();
        return ruleViolation;
    }
}