using Hachodromo.API.Data;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ReservationsController : ControllerBase
{
    private readonly DataContext _context;
    public ReservationsController(DataContext context)
        => _context = context;

    // GET api/reservations
    [HttpGet]
    public async Task<ActionResult<List<ReservationDto>>> GetAll()
    {
        var list = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.ReservationTargets)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                SiteId = r.ReservationTargets.First().TargetId,
                PersonCount = 1, // ajusta si lo almacenas distinto
                Email = r.User != null
                                    ? r.User.Email!
                                    : r.GuestEmail!,
                ReservationDate = r.ReservationDate,
                HourStart = r.HourStart,
                HourEnd = r.HourEnd,
                Remarks = r.Remarks
            })
            .ToListAsync();

        return Ok(list);
    }

    // GET api/reservations/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ReservationDto>> GetById(int id)
    {
        var r = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.ReservationTargets)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (r == null) return NotFound();

        var dto = new ReservationDto
        {
            Id = r.Id,
            SiteId = r.ReservationTargets.First().TargetId,
            PersonCount = 1,
            Email = r.User != null
                                ? r.User.Email!
                                : r.GuestEmail!,
            ReservationDate = r.ReservationDate,
            HourStart = r.HourStart,
            HourEnd = r.HourEnd,
            Remarks = r.Remarks
        };

        return Ok(dto);
    }

  [HttpPost]
[AllowAnonymous]
public async Task<ActionResult<ReservationDto>> Create([FromBody] ReservationDto dto)
{
    if (!ModelState.IsValid)
        return ValidationProblem(ModelState);

    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var emailClaim = User.FindFirstValue(ClaimTypes.Name); // 👈 Correo del usuario logueado

    // Validación para anónimos: Email obligatorio
    if (userIdClaim == null && string.IsNullOrWhiteSpace(dto.Email))
        return BadRequest("El correo es obligatorio para usuarios no autenticados.");

    var entity = new Reservation
    {
        UserId = userIdClaim != null ? Guid.Parse(userIdClaim) : null,
        GuestEmail = userIdClaim != null
                        ? emailClaim   // 👈 Lo guardamos también para autenticados
                        : dto.Email.Trim(),
        ReservationDate = dto.ReservationDate.Date,
        HourStart = dto.HourStart,
        HourEnd = dto.HourEnd,
        Remarks = dto.Remarks,
        CreatedDate = DateTime.UtcNow,

    };

    entity.ReservationTargets.Add(new ReservationTarget
    {
        TargetId = dto.SiteId
    });

    _context.Reservations.Add(entity);
    await _context.SaveChangesAsync();

    dto.Id = entity.Id;
    dto.Email = entity.GuestEmail!;

    return CreatedAtAction(nameof(GetById), new { id = entity.Id }, dto);
}


    // PUT api/reservations/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReservationDto dto)
    {
        if (id != dto.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var entity = await _context.Reservations
            .Include(r => r.ReservationTargets)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity == null)
            return NotFound();

        // Actualizamos campos
        entity.ReservationDate = dto.ReservationDate.Date;
        entity.HourStart = dto.HourStart;
        entity.HourEnd = dto.HourEnd;
        entity.Remarks = dto.Remarks;

        // Si anónimo, permitimos cambiar el GuestEmail
        if (!User.Identity.IsAuthenticated)
            entity.GuestEmail = dto.Email.Trim();

        // Actualizamos target
        var tgt = entity.ReservationTargets.First();
        tgt.TargetId = dto.SiteId;

        await _context.SaveChangesAsync();
        return NoContent();
    }
    //Sumary OBtener las reservas de cada usuario para mostrarlo en pequeñas Cards como Cromos
    [HttpGet("my")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<List<ReservationDto>>> GetMyReservations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return BadRequest("No se ha encontrado el usuario.");
        var userGuid = Guid.Parse(userId);
        var list = await _context.Reservations
            .AsNoTracking()
            .Include(r => r.User)
            .Include(r => r.ReservationTargets)
            .Where(r => r.UserId == userGuid)
            .Select(r => new ReservationDto
            {
                Id = r.Id,
                SiteId = r.ReservationTargets.First().TargetId,
                PersonCount = 1, // ajusta si lo almacenas distinto
                Email = r.User != null
                                    ? r.User.Email!
                                    : r.GuestEmail!,
                ReservationDate = r.ReservationDate,
                HourStart = r.HourStart,
                HourEnd = r.HourEnd,
                Remarks = r.Remarks
            })
            .ToListAsync();
        return Ok(list);
    }
}
