using Hachodromo.API.Data;
using Hachodromo.API.Helpers;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Hachodromo.Shared.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;

namespace Hachodromo.API.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountsController : ControllerBase
    {
        private readonly IUserHelper _userHelper;
        private readonly IConfiguration _configuration;
        private readonly IFileStorage _fileStorage;
        private readonly IMailHelper _mailHelper;
        private readonly string _container;
        private readonly DataContext _context;

        public AccountsController(IUserHelper userHelper, IConfiguration configuration, IFileStorage fileStorage, IMailHelper mailHelper, DataContext context)
        {
            _userHelper = userHelper;
            _configuration = configuration;
            _fileStorage = fileStorage;
            _mailHelper = mailHelper;
            _container = "users";
            _context = context;
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] UserDto model)
        {
            try
            {
                User user = model;

                if (!string.IsNullOrEmpty(model.Photo))
                {
                    var photoUser = Convert.FromBase64String(model.Photo);
                    model.Photo = await _fileStorage.SaveFileAsync(photoUser, ".png", _container);
                }

                var result = await _userHelper.AddUserAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userHelper.AddUserToRole(user, user.UserType.ToString());

                    var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user); //TODO: make a refactor unifiyng this method with the resend token method
                    var tokenLink = $"{_configuration["UrlWeb"]}/confirm-email?userid={user.Id}&token={HttpUtility.UrlEncode(myToken)}";

                    var response = _mailHelper.SendMail(user.FullName, user.Email!,
                        "HachodromoWeb - Confirmación Usuario",
                        $"<h1> Hachodromo - Correo confirmación </h1>" +
                        $"<p>Es necesario activar tu cuenta, haz click en el siguiente enlace para activarla:</p>" +
                        $"<p><a href='{tokenLink}'>Confirmación</a></p>");

                    if (response.IsSuccess)
                    {
                        return NoContent();
                    }

                    return BadRequest(response.Message);
                }

                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                return BadRequest($"Se produjo un error inesperado al crear el usuario: {ex.Message}");
            }
        }


        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginDto model)
        {
            var result = await _userHelper.LoginAsync(model);
            if (result.Succeeded)
            {
                var user = await _userHelper.GetUserAsync(model.Email);
                return Ok(BuildToken(user));
            }
            if(result.IsLockedOut)
            {
                return BadRequest("Usuario bloqueado intente de nuevo en unos minutos");
            }
            if (result.IsNotAllowed)
            {
                return BadRequest("Cuenta no verificada, por favor revise el correo electronico para verificar la cuenta");
            }

                return BadRequest("Usuario o contraseña incorrectos");
        }

        private TokenDto BuildToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email!),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("Document", user.Document),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("Address", user.Address!),
                new Claim("Photo", user.Photo ?? string.Empty),
                new Claim("CityId", user.CityId.ToString()),
                new Claim("BornDate", user.BornDate.ToString("yyyy-MM-dd")),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddDays(7);
            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds);
            return new TokenDto { Token = new JwtSecurityTokenHandler().WriteToken(token), Expiration = expiration };
        }

        [HttpPut]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(User user)
        {
            try
            {
                if (!string.IsNullOrEmpty(user.Photo))
                {
                    var photoUser = Convert.FromBase64String(user.Photo);
                    user.Photo = await _fileStorage.SaveFileAsync(photoUser, ".png", _container);
                }

                var currentUser = await _userHelper.GetUserAsync(user.Email!);
                if (currentUser == null)
                {
                    return NotFound();
                }
                currentUser.FirstName = user.FirstName;
                currentUser.LastName = user.LastName;
                currentUser.Address = user.Address;
                currentUser.CityId = user.CityId;
                currentUser.BornDate = user.BornDate;
                currentUser.Document = user.Document;
                currentUser.UserType = user.UserType;
                currentUser.MembershipId = user.MembershipId;
                currentUser.Photo = !string.IsNullOrEmpty(user.Photo) && user.Photo != currentUser.Photo ? user.Photo : currentUser.Photo;

                var result = await _userHelper.UpdateUserAsync(currentUser);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(currentUser));
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Get()
        {
            return Ok(await _userHelper.GetUserAsync(User.Identity!.Name!));
        }
        [HttpPost("changePassword")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> ChangePasswordAsync(ChangePasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userHelper.GetUserAsync(User.Identity!.Name!);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }

            return NoContent();
        }
        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult> ConfirmEmailAsync(string userId, string token)
        {
            token = token.Replace(" ", "+");
            var user = await _userHelper.GetUserAsync(new Guid(userId));
            if (user == null)
            {
                return NotFound($"No se encontró el usuario con el id {userId}");
            }
            var result = await _userHelper.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }
            return NoContent();
        }
        [HttpPost("ResendToken")]
        public async Task<ActionResult> ResendToken([FromBody] EmailDto model) 
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var myToken = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = $"{_configuration["UrlWeb"]}/confirm-email?userid={user.Id}&token={HttpUtility.UrlEncode(myToken)}";

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                "HachodromoWeb - Confirmación Usuario",
                $"<h1> Hachodromo - Correo confirmación </h1>" +
                $"<p>Es necesario activar tu cuenta, haz click en el siguiente enlace para activarla:</p>" +
                $"<p><a href='{tokenLink}'>Confirmación</a></p>");

            if(response.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
        [HttpPost("RecoverPassword")]
        public async Task<ActionResult> RecoverPassword([FromBody] EmailDto model)
        {
            User user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var myToken = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var tokenLink = $"{_configuration["UrlWeb"]}/resetpassword?userid={user.Id}&token={HttpUtility.UrlEncode(myToken)}";

            var response = _mailHelper.SendMail(user.FullName, user.Email!,
                "HachodromoWeb - Recuperación de contraseña",
                $"<h1> Hachodromo - Recuperación de contraseña </h1>" +
                $"<p>Es necesario recuperar tu contraseña, haz click en el siguiente enlace para recuperarla:</p>" +
                $"<p><a href='{tokenLink}'>Recuperar contraseña</a></p>");

            if (response.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(response.Message);
        }
        [HttpPost("Resetnewpassword")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userHelper.GetUserAsync(model.Email);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.FirstOrDefault()!.Description);
            }
            return NoContent();
        }

        [HttpGet("ByMembership/{membershipId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<User>>> GetUsersByMembership(int membershipId)
        {
            var users = await _userHelper.GetUsersByMembershipAsync(membershipId);

            if (users == null || users.Count == 0)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpPut("RemoveMembership/{userMail}")]
        public async Task<IActionResult> RemoveMembership(string userMail)
        {
            var user = await _userHelper.GetUserAsync(userMail);
            if (user == null)
            { 
                return NotFound();
            }
            user.MembershipId = (int?)MembershipType.NoMembership;
            await _userHelper.UpdateUserAsync(user);
             
            return Ok(user);
        }
        [HttpGet("all")]
        public async Task<ActionResult> GetAll([FromQuery] PaginationDto pagination)
        {
            var quearyable = _context.Users
                .Include(c => c.City)
                .Include(m => m.Membership)
                .AsQueryable();

            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                 quearyable = quearyable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) ||                                                x.LastName.ToLower() == pagination.Filter.ToLower());
            }
            return Ok(await quearyable
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .Paginate(pagination)
                .ToListAsync());
        }
        [HttpGet("totalPages")]
        public async Task<ActionResult> GetPages([FromQuery] PaginationDto pagination)
        {
            var queryable = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.FirstName.ToLower().Contains(pagination.Filter.ToLower()) || x.LastName.ToLower() == pagination.Filter.ToLower());
            }

            double count = await queryable.CountAsync();
            double totalPages = Math.Ceiling(count / pagination.RecordsNumber);
            return Ok(totalPages);
        }

        [HttpGet("{email}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> GetByEmail(string email)
        {
            var user = await _userHelper.GetUserAsync(email);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

    }
}
