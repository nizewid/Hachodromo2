﻿using Hachodromo.API.Data;
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
    [Route("/api/regions")]
    public class RegionsController : ControllerBase
    {
        private readonly DataContext _context;
        public RegionsController(DataContext context)
        {
            _context = context;
        }
        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Regions.Include(x => x.Cities).Where(x => x.Country!.Id == pagination.Id).AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.RegionName.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return Ok(await queryable.OrderBy(x=>x.RegionName).Paginate(pagination).ToListAsync());
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

        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPagesAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Regions.Where(x=>x.Country!.Id == pagination.Id).AsQueryable();

            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.RegionName.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [AllowAnonymous]
        [HttpGet("combo/{countryId:int}")]
        public async Task<IActionResult> GetComboAsync(int countryId)
        {
            return Ok(await _context.Regions
                .Where(x => x.CountryId == countryId)
                .OrderBy(x => x.RegionName)
                //.Select(x => new { x.RegionId, x.RegionName })
                .ToListAsync());
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
