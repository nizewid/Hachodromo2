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
    [Route("api/sites")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]

    public class SitesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public SitesController(DataContext context, IFileStorage fileStorage)
        {
            _context = context;
            _fileStorage = fileStorage;
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<Site>>> Get([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Sites
                .Include(c => c.City!)
                .ThenInclude(r => r.Region!)
                .ThenInclude(c => c.Country!)
                .Include(t => t.Targets!)
                .AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return Ok(await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync());
        }
        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPagesAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Sites.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Site>> Get(int id)
        {
            var site = await _context.Sites
                .Include(c => c.City!)
                .ThenInclude(r => r.Region!)
                .ThenInclude(c => c.Country!)
                .Include(t => t.Targets!)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (site == null)
            {
                return NotFound();
            }
            return Ok(site);
        }
        [HttpPost]
        public async Task<ActionResult<Site>> Post([FromBody] SiteDto siteDto)
        {
            try
            {
                var site = new Site
                {
                    Name = siteDto.Name,
                    Description = siteDto.Description,
                    CityId = siteDto.CityId,
                    Address = siteDto.Address,
                    Phone = siteDto.Phone,
                    Targets = Enumerable.Range(0, siteDto.TargetsToCreate.Value)
                        .Select(i => new Target
                        {
                            Capacity = 6,
                            Status = Shared.Enums.TargetStatus.Available,
                            // Aquí puedes agregar más propiedades según sea necesario
                        }).ToList()
                };
                _context.Sites.Add(site);
                await _context.SaveChangesAsync();
                return Ok(site);
            }
            catch (DbUpdateException dbUpdateException)
            {
                if (dbUpdateException.InnerException!.Message.Contains("duplicate"))
                {
                    return BadRequest("Ya existe un Item con ese nombre");
                }
                return BadRequest(dbUpdateException.InnerException.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("combo")]
        [AllowAnonymous]
        public async Task<ActionResult<List<Site>>> GetComboAsync()
        {
            var sites = await _context.Sites
                .Include(c => c.City!)
                .ThenInclude(r => r.Region!)
                .ThenInclude(c => c.Country!)
                .ToListAsync();
            return Ok(sites);
        }

        // GET api/sites/5/timeslots?date=2025-05-07
        [HttpGet("{siteId:int}/timeslots")]
        [AllowAnonymous]
        public async Task<ActionResult<List<TimeSlotDto>>> GetTimeSlots(
            int siteId,
            [FromQuery] DateTime date)
        {
            // 1) Número total de blancos en el sitio
            var totalTargets = await _context.Targets
                .CountAsync(t => t.SiteId == siteId);

            // 2) Cargamos todas las reservas de ese sitio y fecha
            var reservationsOnDate = await _context.ReservationTargets
                .Include(rt => rt.Reservation)
                .Include(rt => rt.Target)
                .Where(rt =>
                    rt.Target.SiteId == siteId &&
                    rt.Reservation.ReservationDate.Date == date.Date)
                .ToListAsync();

            // 3) Generamos los slots de 12–23h
            var slots = Enumerable.Range(12, 11)
                .Select(h =>
                {
                    var start = TimeSpan.FromHours(h);
                    var end = start.Add(TimeSpan.FromHours(1));
                    // cuántos blancos ya están reservados en ese slot
                    var reservedCount = reservationsOnDate
                        .Count(rt => rt.Reservation.HourStart == start);
                    return new TimeSlotDto
                    {
                        Start = start,
                        End = end,
                        AvailableCount = totalTargets - reservedCount
                    };
                })
                .ToList();

            return Ok(slots);
        }
    }
}

