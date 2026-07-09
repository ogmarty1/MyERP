using ERP.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ERP.DataAccess.Data
{
    public static class DataSeeder
    {
        public const string AdminUsername = "admin";

        // Dev-only default password for the seeded administrator; change on first login in a real deployment.
        private const string DefaultAdminPassword = "Admin@12345";

        public static async Task SeedAsync(ApplicationDbContext context)
        {
            var roleNames = new[] { "Admin", "Manager", "Employee" };

            foreach (var roleName in roleNames)
            {
                var roleExists = await context.Roles.AnyAsync(r => r.Name == roleName);
                if (!roleExists)
                {
                    context.Roles.Add(new Role { Name = roleName });
                }
            }

            await context.SaveChangesAsync();

            var adminUser = await context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Username == AdminUsername);

            if (adminUser == null)
            {
                adminUser = new User
                {
                    Username = AdminUsername,
                    Email = "admin@myerp.local",
                    IsActive = true
                };

                var hasher = new PasswordHasher<User>();
                adminUser.PasswordHash = hasher.HashPassword(adminUser, DefaultAdminPassword);

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }

            var adminHasRole = await context.UserRoles
                .AnyAsync(ur => ur.UserId == adminUser.Id && ur.Role.Name == "Admin");

            if (!adminHasRole)
            {
                var adminRole = await context.Roles.FirstAsync(r => r.Name == "Admin");
                context.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });
                await context.SaveChangesAsync();
            }
        }
    }
}
