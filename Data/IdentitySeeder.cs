using FerticaStockManagement.Models;
using Microsoft.AspNetCore.Identity;

namespace FerticaStockManagement.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // ========================================
            // CREATE ROLES
            // ========================================

            string[] roles =
            {
                "Administrator",
                "Employee"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role)
                    );
                }
            }

            // ========================================
            // ADMINISTRATOR
            // ========================================

            // Check if an administrator already exists
            var administrators = await userManager.GetUsersInRoleAsync(
                "Administrator"
            );

            if (administrators.Count == 0)
            {
                // Create the FIRST administrator
                var admin = new ApplicationUser
                {
                    UserName = "admin@fertica.local",
                    Email = "admin@fertica.local",
                    FirstName = "System",
                    LastName = "Administrator",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    admin,
                    "Admin123!"
                );

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(
                        admin,
                        "Administrator"
                    );
                }
            }
        }
    }
}