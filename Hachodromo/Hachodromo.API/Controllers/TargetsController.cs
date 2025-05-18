using Hachodromo.API.Data;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Controllers
{
    [ApiController]
    [Route("api/sites/{siteId:int}/targets")]
    public class TargetsController : ControllerBase
    {
        private readonly DataContext _context;
        public TargetsController(DataContext context) => _context = context;

        // GET api/sites/2/targets
        [HttpGet]
        public async Task<ActionResult<List<TargetDto>>> GetBySite(int siteId)
        {
            var list = await _context.Targets
                .AsNoTracking()
                .Where(t => t.SiteId == siteId)
                .Select(t => new TargetDto
                {
                    Id = t.Id,
                    SiteId = t.SiteId,
                    Capacity = t.Capacity,
                    Status = t.Status
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST api/sites/2/targets
        [HttpPost]
        public async Task<ActionResult<TargetDto>> Post(int siteId, TargetDto targetDto)
        {
            var site = await _context.Sites.FindAsync(siteId);
            if (site == null) return NotFound();

            var target = new Target
            {
                SiteId = siteId,
                Capacity = targetDto.Capacity,
                Status = targetDto.Status
            };

            _context.Targets.Add(target);
            await _context.SaveChangesAsync();

            targetDto.Id = target.Id;
            return CreatedAtAction(nameof(GetBySite), new { siteId }, targetDto);
        }

        // PUT api/sites/2/targets/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int siteId, int id, TargetDto targetDto)
        {
            if (siteId != targetDto.SiteId || id != targetDto.Id)
                return BadRequest();

            var target = await _context.Targets.FindAsync(id);
            if (target == null) return NotFound();

            target.Capacity = targetDto.Capacity;
            target.Status = targetDto.Status;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/sites/2/targets/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int siteId, int id)
        {
            var target = await _context.Targets
                .FirstOrDefaultAsync(t => t.SiteId == siteId && t.Id == id);

            if (target == null) return NotFound();

            _context.Targets.Remove(target);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
