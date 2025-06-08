using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(PlanetEvaluateDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Check if users already exist
            if (await context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Create default users
            var users = new List<User>
            {
                new User
                {
                    UserName = "superadmin",
                    Email = "superadmin@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    FirstName = "Super",
                    LastName = "Admin",
                    Role = "superadmin",
                    AssignedPlanetIds = null, // SuperAdmin has access to all
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    UserName = "planetadmin",
                    Email = "planetadmin@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    FirstName = "Planet",
                    LastName = "Admin",
                    Role = "planetadmin",
                    AssignedPlanetIds = "[1,2,3]", // JSON array of planet IDs
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    UserName = "viewer1",
                    Email = "viewer1@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    FirstName = "Viewer",
                    LastName = "One",
                    Role = "viewer",
                    AssignedPlanetIds = "[1,2]", // JSON array of planet IDs
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    UserName = "viewer2",
                    Email = "viewer2@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    FirstName = "Viewer",
                    LastName = "Two",
                    Role = "viewer",
                    AssignedPlanetIds = "[2,3]", // JSON array of planet IDs
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
