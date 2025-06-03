using Hachodromo.API.Data;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Hachodromo.Shared.Enums;
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

        // Si no está autenticado, obligamos a que llegue correo
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var emailClaim = User.FindFirstValue(ClaimTypes.Name);
        if (userIdClaim == null && string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("El correo es obligatorio para usuarios no autenticados.");

        // Fecha y hora de inicio de la reserva (solo fecha, ignoramos hora de fin para buscar diana)
        var fecha = dto.ReservationDate.Date;
        var horaInicio = dto.HourStart;

        // 1) Calculamos qué dianas de ese sitio están marcadas como Available
        //    y NO tienen ya una ReservationTarget para la misma fecha+hora.
        //    Si no queda ninguna, devolvemos BadRequest.
        var dianaLibreId = await _context.Targets
            .Where(t => t.SiteId == dto.SiteId && t.Status == TargetStatus.Available)
            .Select(t => t.Id)
            // Excluir las que ya están en ReservationTargets en esa fecha+hInicio
            .Except(
                _context.ReservationTargets
                    .Include(rt => rt.Reservation)
                    .Where(rt =>
                        rt.Target.SiteId == dto.SiteId &&
                        rt.Reservation.ReservationDate == fecha &&
                        rt.Reservation.HourStart == horaInicio
                    )
                    .Select(rt => rt.TargetId)
            )
            .FirstOrDefaultAsync();

        if (dianaLibreId == 0)
            return BadRequest("No hay dianas disponibles para la fecha y hora seleccionadas.");

        // 2) Creamos la entidad Reservation
        var entity = new Reservation
        {
            UserId = userIdClaim != null ? Guid.Parse(userIdClaim) : null,
            GuestEmail = userIdClaim != null
                            ? emailClaim   // Guardamos correo de usuario autenticado
                            : dto.Email.Trim(),
            ReservationDate = fecha,
            HourStart = horaInicio,
            HourEnd = dto.HourEnd,
            Remarks = dto.Remarks,
            CreatedDate = DateTime.UtcNow
        };

        // 3) Asociamos UNA sola ReservationTarget con la diana elegida
        entity.ReservationTargets.Add(new ReservationTarget
        {
            TargetId = dianaLibreId
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
    // DELETE api/reservations/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.Reservations
            .Include(r => r.ReservationTargets)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (entity == null)
            return NotFound();

        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userRole = User.FindFirstValue(ClaimTypes.Role);

        var isOwner = entity.UserId != null && entity.UserId.ToString() == userIdClaim;
        var isAdmin = userRole == UserType.Admin.ToString();

        if (!isOwner && !isAdmin)
            return Forbid();

        _context.ReservationTargets.RemoveRange(entity.ReservationTargets);
        _context.Reservations.Remove(entity);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
