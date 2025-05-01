using Hachodromo.API.Data;
using Hachodromo.API.Helpers;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/countries")]
    public class CountriesControllers : ControllerBase
    {
        private readonly DataContext _context;
        public CountriesControllers(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<ActionResult<List<Country>>> GetAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Countries
                .Include(x => x.Regions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            return Ok(await queryable
                .OrderBy(x => x.Name)
                .Paginate(pagination)
                .ToListAsync());
        }


        [HttpGet("totalPages")]
        public async Task<ActionResult<double>> GetPagesAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Countries.AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [AllowAnonymous]
        [HttpGet("combo")]
        public async Task<ActionResult<List<Country>>> GetComboAsync()
        {
            return Ok(await _context.Countries
                .OrderBy(x => x.Name)
             //   .Select(x => new { x.Id, x.Name })
                .ToListAsync());
        }

        [HttpGet("/full")]
        public async Task<ActionResult<List<Country>>> GetFullAsync()
        {
            return Ok(await _context.Countries
                .Include(x => x.Regions!)
                .ThenInclude(x=>x.Cities)
                .ToListAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Country>> GetAsync(int id)
        {
            var country = await _context.Countries
                .Include(x=>x.Regions!)
                .ThenInclude(x=> x.Cities)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            return Ok(country);
        }
        [HttpPost]
        public async Task<ActionResult<Country>> PostAsync(Country country)
        {
            try
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un país con ese nombre");
                }
                return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<ActionResult<Country>> PutAsync(Country country)
        {
            try
            {
                _context.Update(country);
                await _context.SaveChangesAsync();
                return Ok(country);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un país con ese nombre");
                }
                return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Country>> DeleteAsync(int id)
        {
            var country = await _context.Countries.FirstOrDefaultAsync(x => x.Id == id);
            if (country == null)
            {
                return NotFound();
            }
            _context.Remove(country);
            await _context.SaveChangesAsync();
            return Ok(country);
        }
    }
}
