using Hachodromo.API.Data;
using Hachodromo.API.Helpers;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Hachodromo.Shared.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hachodromo.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("/api/memberships")]
    [Authorize(Roles = "Admin")]
    public class MembershipsController : ControllerBase
    {
        private readonly DataContext _context;

        public MembershipsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Membership>>> GetAsync([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Memberships.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return Ok(await queryable
                .OrderBy(x => x.Discount)
                .Paginate(pagination)
                .ToListAsync());
        }
        [HttpGet("totalPages")]
        public async Task<IActionResult> GetPages([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Memberships.AsQueryable();
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Membership>> GetAsync(int id)
        {
            var membership = await _context.Memberships
                .FirstOrDefaultAsync(x => x.Id == id);
            if (membership is null)
            {
                return NotFound();
            }
            return Ok(membership);
        }
        [HttpPost]
        public async Task<ActionResult> PostAsync(Membership membership)
        {
            _context.Add(membership);
            await _context.SaveChangesAsync();
            return Ok(membership);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutAsync(int id, Membership membership)
        {
            if (id != membership.Id)
            {
                return BadRequest("Membership ID mismatch");
            }
            _context.Update(membership);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            if (id == (int)MembershipType.NoMembership)
            {
                return BadRequest("No se puede eliminar la membresía por defecto.");
            }
            var membership = await _context.Memberships
                .FirstOrDefaultAsync(x => x.Id == id);
            if (membership is null)
            {
                return NotFound();
            }

            var usersWithMembership = await _context.Users.Where(u => u.MembershipId == id).ToListAsync();

            foreach (var user in usersWithMembership)
            {
                user.MembershipId = (int?)MembershipType.NoMembership;
            }
            _context.Remove(membership);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
