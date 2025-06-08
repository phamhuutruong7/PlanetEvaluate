using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Data
{
    public static class DatabaseSeeder
    {        public static async Task SeedAsync(PlanetEvaluateDbContext context)
        {
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();

            // Seed users if they don't exist
            if (!await context.Users.AnyAsync())
            {
                // Create default users
                var users = new List<User>
                {                    new User
                    {
                        UserName = "superadmin",
                        Email = "superadmin@planetevaluate.com",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                        FirstName = "Super",
                        LastName = "Admin",
                        Role = "SuperAdmin",
                        AssignedPlanetIds = null, // SuperAdmin has access to all
                        CreatedAt = DateTime.UtcNow
                    },                new User
                {
                    UserName = "planetadmin",
                    Email = "planetadmin@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    FirstName = "Planet",
                    LastName = "Admin",
                    Role = "PlanetAdmin",
                    AssignedPlanetIds = "[1,2,3,4,5]", // JSON array of planet IDs - has access to first 5 planets
                    CreatedAt = DateTime.UtcNow
                },                new User
                {
                    UserName = "viewer1",
                    Email = "viewer1@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
                    FirstName = "Viewer",
                    LastName = "Type One",
                    Role = "Viewer1", // Same permissions as Viewer2 but different planet access
                    AssignedPlanetIds = "[1]", // JSON array - access to planet 1 only
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    UserName = "viewer2",
                    Email = "viewer2@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("viewer123"),
                    FirstName = "Viewer",
                    LastName = "Type Two",
                    Role = "Viewer2", // Same permissions as Viewer1 but different planet access
                    AssignedPlanetIds = "[1,3]", // JSON array - access to planets 1 and 3
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    UserName = "viewergeneric",
                    Email = "viewergeneric@planetevaluate.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
                    FirstName = "Generic",
                    LastName = "Viewer",
                    Role = "Viewer2", // Generic viewer using assigned planets
                    AssignedPlanetIds = "[2,4,5]", // JSON array of planet IDs - has access to specific planets
                    CreatedAt = DateTime.UtcNow
                }
                };

                await context.Users.AddRangeAsync(users);
                await context.SaveChangesAsync();
            }            // Seed planets if they don't exist
            if (!await context.Planets.AnyAsync())
            {
                // Create planets from CSV data
                var planets = new List<Planet>
                {
                    new Planet
                    {
                        Name = "PlanetAdminUpdate",
                        Type = "Test",
                        Mass = 1,
                        Radius = 1,
                        DistanceFromSun = 1,
                        NumberOfMoons = 0,
                        HasAtmosphere = false,
                        OxygenVolume = 0,
                        WaterVolume = 0,
                        HardnessOfRock = 5,
                        ThreateningCreature = 1,
                        Description = "PlanetAdmin update test",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Venus",
                        Type = "Terrestrial",
                        Mass = 0.815,
                        Radius = 0.949,
                        DistanceFromSun = 0.72,
                        NumberOfMoons = 0,
                        HasAtmosphere = true,
                        OxygenVolume = 0,
                        WaterVolume = 0,
                        HardnessOfRock = 6,
                        ThreateningCreature = 9,
                        Description = "Second planet from the Sun, known for its thick, toxic atmosphere.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Earth",
                        Type = "Terrestrial",
                        Mass = 1,
                        Radius = 1,
                        DistanceFromSun = 1,
                        NumberOfMoons = 1,
                        HasAtmosphere = true,
                        OxygenVolume = 21,
                        WaterVolume = 71,
                        HardnessOfRock = 5,
                        ThreateningCreature = 6,
                        Description = "Third planet from the Sun and the only known planet to harbor life.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Mars",
                        Type = "Terrestrial",
                        Mass = 0.107,
                        Radius = 0.532,
                        DistanceFromSun = 1.52,
                        NumberOfMoons = 2,
                        HasAtmosphere = true,
                        OxygenVolume = 0.13,
                        WaterVolume = 0,
                        HardnessOfRock = 7,
                        ThreateningCreature = 3,
                        Description = "Fourth planet from the Sun, known as the Red Planet.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Jupiter",
                        Type = "Gas Giant",
                        Mass = 317.8,
                        Radius = 11.21,
                        DistanceFromSun = 5.2,
                        NumberOfMoons = 95,
                        HasAtmosphere = true,
                        OxygenVolume = 0,
                        WaterVolume = 0,
                        HardnessOfRock = 1,
                        ThreateningCreature = 10,
                        Description = "Fifth planet from the Sun and the largest in the Solar System.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Saturn",
                        Type = "Gas Giant",
                        Mass = 95.2,
                        Radius = 9.45,
                        DistanceFromSun = 9.5,
                        NumberOfMoons = 146,
                        HasAtmosphere = true,
                        OxygenVolume = 0,
                        WaterVolume = 0,
                        HardnessOfRock = 1,
                        ThreateningCreature = 9,
                        Description = "Sixth planet from the Sun, famous for its prominent ring system.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Uranus",
                        Type = "Ice Giant",
                        Mass = 14.5,
                        Radius = 4.01,
                        DistanceFromSun = 19.2,
                        NumberOfMoons = 27,
                        HasAtmosphere = true,
                        OxygenVolume = 0,
                        WaterVolume = 80,
                        HardnessOfRock = 2,
                        ThreateningCreature = 5,
                        Description = "Seventh planet from the Sun, rotates on its side.",
                        CreatedAt = DateTime.UtcNow
                    },
                    new Planet
                    {
                        Name = "Neptune",
                        Type = "Ice Giant",
                        Mass = 17.1,
                        Radius = 3.88,
                        DistanceFromSun = 30.1,
                        NumberOfMoons = 14,
                        HasAtmosphere = true,
                        OxygenVolume = 0,
                        WaterVolume = 85,
                        HardnessOfRock = 2,
                        ThreateningCreature = 7,
                        Description = "Eighth and outermost planet from the Sun, known for its strong winds.",
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Planets.AddRangeAsync(planets);
                await context.SaveChangesAsync();
            }
        }
    }
}
