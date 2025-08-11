using Microsoft.AspNetCore.Identity;
using MovieFlix.Models;

namespace MovieFlix.Data
{
    public static class DbInitializer
    {
        public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "SuperAdmin", "CinemaAdmin", "User" };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // SuperAdmin
            var superAdminEmail = "superadmin@movies.com";
            if (await userManager.FindByEmailAsync(superAdminEmail) == null)
            {
                var superAdmin = new ApplicationUser
                {
                    UserName = superAdminEmail,
                    Email = superAdminEmail,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(superAdmin, "SuperAdmin");
                await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
            }

            // Cinema Admins
            for (int i = 1; i <= 3; i++)
            {
                var adminEmail = $"cinema{i}@movies.com";
                if (await userManager.FindByEmailAsync(adminEmail) == null)
                {
                    var cinemaAdmin = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true,
                        CinemaId = i
                    };
                    await userManager.CreateAsync(cinemaAdmin, "Admin@123");
                    await userManager.AddToRoleAsync(cinemaAdmin, "CinemaAdmin");
                }
            }
        }
    }
}
