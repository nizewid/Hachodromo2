using Hachodromo.API.Data;
using Hachodromo.Shared.DTOs;
using Hachodromo.Shared.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Hachodromo.API.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly DataContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _singInManager;

        public UserHelper(DataContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> singInManager)
        {
            UserManager = userManager;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _singInManager = singInManager;
        }

        public UserManager<User> UserManager { get; }

        public async Task<IdentityResult> AddUserAsync(User user, string password)
        {
            return await _userManager.CreateAsync(user, password);
        }

        public async Task AddUserToRole(User user, string roleName)
        {
            await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> ChangePasswordAsync(User user, string oldPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
        }

        public async Task CheckRoleAsync(string roleName)
        {
            bool roleExists = await _roleManager.RoleExistsAsync(roleName);
            if(!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
        {
            return await _userManager.ConfirmEmailAsync(user, token);
        }

        public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
        {
            return await _userManager.GenerateEmailConfirmationTokenAsync(user);
        }


        public async Task<User> GetUserAsync(string email)
        {
            var user = await _context.Users
                                        .Include(c => c.City!)
                                        .ThenInclude(r => r.Region!)
                                        .ThenInclude(c => c.Country!)
                                        .Include(m => m.Membership!)
                                        .FirstOrDefaultAsync(x => x.Email == email);
            return user!;
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            var user = await _context.Users
                                        .Include(c => c.City!)
                                        .ThenInclude(r => r.Region!)
                                        .ThenInclude(c => c.Country!)
                                        .Include(m => m.Membership!)
                                        .FirstOrDefaultAsync(x => x.Id == userId.ToString());
            return user!;
        }

        public async Task<bool> IsUserInRoleAsync(User user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<SignInResult> LoginAsync(LoginDto model)
        {
            return await _singInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
        }

        public async Task LogoutAsync()
        {
            await _singInManager.SignOutAsync();
        }
        public async Task<IdentityResult> UpdateUserAsync(User user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }


        public async Task<string> GeneratePasswordResetTokenAsync(User user)
        {
            return await _userManager.GeneratePasswordResetTokenAsync(user);
        }

        public async Task<List<User>> GetUsersByMembershipAsync(int membershipId)
        {
            return await _context.Users
                .Include(c => c.City!)
                .ThenInclude(r => r.Region!)
                .ThenInclude(c => c.Country!)
                .Include(m => m.Membership!)
                .Where(u => u.MembershipId == membershipId)
                .ToListAsync();
        }
    }
}
