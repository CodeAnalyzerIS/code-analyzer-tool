using CAS_Backend.Dtos;
using CAS_Backend.Managers;
using Microsoft.AspNetCore.Mvc;

namespace CAS_Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {
        private readonly IAnalysisManager _manager;

        public AnalysisController(IAnalysisManager manager)
        {
            _manager = manager;
        }

        // // GET: api/Analysis
        // [HttpGet]
        // public async Task<ActionResult<IEnumerable<AnalysisResultDto>>> GetAnalysisResults()
        // {
        //   return 
        // }

        // // GET: api/Analysis/5
        // [HttpGet("{id}")]
        // public async Task<ActionResult<AnalysisResultDto>> GetAnalysisResult(int id)
        // {
        //   if (_context.AnalysisResults == null)
        //   {
        //       return NotFound();
        //   }
        //     var analysisResult = await _context.AnalysisResults.FindAsync(id);
        //
        //     if (analysisResult == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     return analysisResult;
        // }

        // // PUT: api/Analysis/5
        // // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // [HttpPut("{id}")]
        // public async Task<IActionResult> PutAnalysisResult(int id, AnalysisResultDto analysisResult)
        // {
        //     if (id != analysisResult.Id)
        //     {
        //         return BadRequest();
        //     }
        //
        //     _context.Entry(analysisResult).State = EntityState.Modified;
        //
        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         if (!AnalysisResultExists(id))
        //         {
        //             return NotFound();
        //         }
        //         else
        //         {
        //             throw;
        //         }
        //     }
        //
        //     return NoContent();
        // }

        // POST: api/Analysis
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AnalysisResultDto>> PostAnalysisResult(AnalysisResultDto analysisResult)
        {
            Console.WriteLine(analysisResult.Message);
            return analysisResult;
        }

        // // DELETE: api/Analysis/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteAnalysisResult(int id)
        // {
        //     if (_context.AnalysisResults == null)
        //     {
        //         return NotFound();
        //     }
        //     var analysisResult = await _context.AnalysisResults.FindAsync(id);
        //     if (analysisResult == null)
        //     {
        //         return NotFound();
        //     }
        //
        //     _context.AnalysisResults.Remove(analysisResult);
        //     await _context.SaveChangesAsync();
        //
        //     return NoContent();
        // }
        //
        // private bool AnalysisResultExists(int id)
        // {
        //     return (_context.AnalysisResults?.Any(e => e.Id == id)).GetValueOrDefault();
        // }
    }
}
