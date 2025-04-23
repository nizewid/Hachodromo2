using Hachodromo.API.Data;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Controllers
{
    [ApiController]
    [Route("/api/regions")]
    public class RegionsController : ControllerBase
    {
        private readonly DataContext _context;
        public RegionsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            return Ok(await _context.Regions
                .Include(x => x.Cities)
                .ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            {
                var region = await _context.Regions
                    .Include(x=>x.Cities)
                    .FirstOrDefaultAsync(x => x.RegionId == id);
                if (region== null)
                {
                    return NotFound();
                }
                return Ok(region);
            }
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(Region region)
        {
            try
            {
                _context.Add(region);
                await _context.SaveChangesAsync();
                return Ok(region);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe una región con ese nombre");
                }
                return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult> PutAsync(Region region)
        {
            try
            {
                _context.Update(region);
                await _context.SaveChangesAsync();
                return Ok(region);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un Estado con ese nombre");
                }
                return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var region = await _context.Regions.FirstOrDefaultAsync(x => x.RegionId == id);
            if (region == null)
            {
                return NotFound();
            }
            _context.Remove(region);
            await _context.SaveChangesAsync();
            return Ok(region);
        }
    }
}
