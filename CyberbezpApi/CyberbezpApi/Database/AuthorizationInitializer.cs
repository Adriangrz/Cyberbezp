using CyberbezpApi.Database.Entities;
using Microsoft.AspNetCore.Identity;

namespace CyberbezpApi.Database
{
    public interface IAuthorizationInitializer
    {
        Task GenerateAdminAndRoles();
    }
    public class AuthorizationInitializer : IAuthorizationInitializer
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleMannager;
        public AuthorizationInitializer(SignInManager<User> signInManager, UserManager<User> userManager, RoleManager<IdentityRole> roleMannager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleMannager = roleMannager;
        }
        public async Task GenerateAdminAndRoles()
        {
            string adminRole = "Admin";
            string userRole = "User";

            if (!await _roleMannager.RoleExistsAsync(adminRole))
            {
                await _roleMannager.CreateAsync(new IdentityRole(adminRole));
            }

            if (!await _roleMannager.RoleExistsAsync(userRole))
            {
                await _roleMannager.CreateAsync(new IdentityRole(userRole));
            }

            var admin = new User()
            {
                SecurityStamp = new Guid().ToString(),
                UserName = "ADMIN",
                Email = "admin@test.pl",
            };

            if (await _userManager.FindByNameAsync(admin.UserName) == null)
            {
                await _userManager.CreateAsync(admin, "Pass4Admin!");
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(admin);
                await _userManager.ConfirmEmailAsync(admin, code);
                await _userManager.AddToRoleAsync(admin, adminRole);
            }
        }
    }
}
