using Hachodromo.API.Data;
using Hachodromo.Shared.DTOs;
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
                .Where(t => t.Status == Shared.Enums.TargetStatus.Available)
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
    }

}
