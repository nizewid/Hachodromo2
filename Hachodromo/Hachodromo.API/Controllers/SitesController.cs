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
            //Número total de blancos en el sitio disponibles
            var totalTargets = await _context.Targets.Where(t => t.SiteId == siteId && t.Status == Shared.Enums.TargetStatus.Available)
                .CountAsync();


            //todas las reservas de ese sitio y fecha
            var reservationsOnDate = await _context.ReservationTargets
                .Include(rt => rt.Reservation)
                .Include(rt => rt.Target)
                .Where(rt =>
                    rt.Target.SiteId == siteId &&
                    rt.Reservation.ReservationDate.Date == date.Date)
                .ToListAsync();

            //Generamos los slots de 12–23h
            var slots = Enumerable.Range(12, 11)
                .Select(h =>
                {
                    var start = TimeSpan.FromHours(h);
                    var end = start.Add(TimeSpan.FromHours(1));

                    var reservedCount = reservationsOnDate
                        .Where(rt => rt.Reservation.HourStart == start)
                        .Count(); // cuenta dianas ocupadas, no reservas

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
        /// <summary>
        /// Devuelve todas las reservas de un sitio dentro de un rango de fechas
        /// (incluye ambas fechas). Ideal para las vistas Semana y Mes.
        ///     GET api/sites/5/reservations/range?from=2025-06-02&to=2025-06-08
        /// </summary>
        /// <param name="siteId">Identificador numérico del sitio</param>
        /// <param name="from">Fecha de inicio del rango (inclusive)</param>
        /// <param name="to">Fecha fin del rango (inclusive)</param>
        /// <returns>Lista de <see cref="ReservationDto"/> ordenada por fecha y hora</returns>
        [HttpGet("{siteId:int}/reservations/range")]
        [AllowAnonymous]                 // cámbialo a [Authorize] si sólo para admin
        public async Task<ActionResult<List<ReservationDto>>> GetReservationsRange(
            int siteId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (to < from)
                return BadRequest("La fecha 'to' debe ser mayor o igual que 'from'.");

            var list = await _context.ReservationTargets
                .Include(rt => rt.Reservation)
                .Where(rt => rt.Target.SiteId == siteId &&
                             rt.Reservation.ReservationDate >= from.Date &&
                             rt.Reservation.ReservationDate <= to.Date)
                .Select(rt => rt.Reservation)
                .Distinct()                          // evita duplicados por Target
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.HourStart)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    SiteId = siteId,
                    PersonCount = r.ReservationTargets.Count,
                    Email = r.User != null ? r.User.Email! : r.GuestEmail!,
                    ReservationDate = r.ReservationDate,
                    HourStart = r.HourStart,
                    HourEnd = r.HourEnd,
                    Remarks = r.Remarks
                })
                .ToListAsync();

            return Ok(list);
        }
        /// <summary>
        /// Devuelve las reservas de un sitio para un día concreto o para un rango de fechas.
        /// Ejemplos:
        ///   GET api/sites/5/reservations?date=2025-06-03
        ///   GET api/sites/5/reservations?startDate=2025-06-01&endDate=2025-06-07
        /// Si no se especifica ni <c>date</c> ni <c>startDate/endDate</c>, devuelve todas las reservas del sitio.
        /// </summary>
        /// <param name="siteId">Id del sitio</param>
        /// <param name="date">Fecha única a consultar (yyyy-MM-dd)</param>
        /// <param name="startDate">Fecha de inicio del rango (yyyy-MM-dd)</param>
        /// <param name="endDate">Fecha de fin del rango (yyyy-MM-dd)</param>
        [HttpGet("{siteId:int}/reservations")]
        [AllowAnonymous]  // Cambiar a [Authorize] si se necesita autenticación
        public async Task<ActionResult<List<ReservationDto>>> GetReservationsBySite(
            int siteId,
            [FromQuery] DateTime? date,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            // 1) Base query: todas las reservas cuyo Target pertenece al sitio
            var query = _context.ReservationTargets
                                .Include(rt => rt.Reservation)
                                .ThenInclude(r => r.ReservationTargets)
                                .ThenInclude(rt => rt.Target)
                                .Include(rt => rt.Reservation)
                                .ThenInclude(r => r.User)
                                .Where(rt => rt.Target.SiteId == siteId)
                                .Select(rt => rt.Reservation)
                                .Distinct();

            // 2) Filtro por rango o por día
            if (startDate.HasValue && endDate.HasValue)
            {
                var inicio = startDate.Value.Date;
                var fin = endDate.Value.Date;
                query = query.Where(r => r.ReservationDate >= inicio && r.ReservationDate <= fin);
            }
            else if (date.HasValue)
            {
                var dia = date.Value.Date;
                query = query.Where(r => r.ReservationDate == dia);
            }

            // 3) Proyección a DTO ordenada por fecha y hora de inicio
            var list = await query
                .OrderBy(r => r.ReservationDate)
                .ThenBy(r => r.HourStart)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    SiteId = siteId,
                    PersonCount = r.ReservationTargets.Count,
                    Email = r.User != null ? r.User.Email! : r.GuestEmail!,
                    ReservationDate = r.ReservationDate,
                    HourStart = r.HourStart,
                    HourEnd = r.HourEnd,
                    Remarks = r.Remarks
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}

