using CodeAnalyzerService.Backend.DAL.EF;
using CodeAnalyzerService.Backend.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CodeAnalyzerService.Backend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RuleController : ControllerBase
{
    private readonly CodeAnalyzerServiceDbContext _context;

    public RuleController(CodeAnalyzerServiceDbContext context)
    {
        _context = context;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RuleDetailsResponse>> GetRule(int id)
    {
        var rule = await _context.Rules.Where(r => r.Id == id)
            .Select(r => new RuleDetailsResponse(
                    r.Id,
                    r.RuleName,
                    r.Title,
                    r.Description,
                    r.Category,
                    r.PluginName,
                    r.TargetLanguage,
                    r.CodeExample,
                    r.CodeExampleFix
                    )
            ).SingleOrDefaultAsync();
        if (rule == null) return NotFound();
        return rule;
    }
}