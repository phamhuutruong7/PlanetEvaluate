using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Data;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using System.Text.Json;

namespace PlanetEvaluateApi.Services
{
    public class PlanetService : IPlanetService
    {
        private readonly PlanetEvaluateDbContext _context;

        public PlanetService(PlanetEvaluateDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Planet>> GetAllPlanetsAsync(int? userId = null)
        {
            var query = _context.Planets.AsQueryable();

            // If user is specified, filter by user's assigned planets
            if (userId.HasValue)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null && user.Role != "superadmin")
                {
                    var assignedPlanetIds = GetUserAssignedPlanetIds(user);
                    if (assignedPlanetIds.Any())
                    {
                        query = query.Where(p => assignedPlanetIds.Contains(p.Id));
                    }
                    else
                    {
                        // User has no assigned planets, return empty list
                        return new List<Planet>();
                    }
                }
            }

            return await query.OrderBy(p => p.Name).ToListAsync();
        }

        public async Task<Planet?> GetPlanetByIdAsync(int id, int? userId = null)
        {
            var planet = await _context.Planets.FindAsync(id);
            
            if (planet == null)
                return null;

            // Check access if user is specified
            if (userId.HasValue && !await UserHasAccessToPlanetAsync(userId.Value, id))
            {
                return null; // User doesn't have access to this planet
            }

            return planet;
        }

        public async Task<Planet> CreatePlanetAsync(Planet planet)
        {
            planet.CreatedAt = DateTime.UtcNow;
            planet.UpdatedAt = DateTime.UtcNow;
            
            _context.Planets.Add(planet);
            await _context.SaveChangesAsync();
            
            return planet;
        }

        public async Task<Planet?> UpdatePlanetAsync(int id, Planet planet, int? userId = null)
        {
            var existingPlanet = await _context.Planets.FindAsync(id);
            if (existingPlanet == null)
                return null;

            // Check access if user is specified
            if (userId.HasValue && !await UserHasAccessToPlanetAsync(userId.Value, id))
            {
                return null; // User doesn't have access to this planet
            }

            // Update properties
            existingPlanet.Name = planet.Name;
            existingPlanet.Type = planet.Type;
            existingPlanet.Mass = planet.Mass;
            existingPlanet.Radius = planet.Radius;
            existingPlanet.DistanceFromSun = planet.DistanceFromSun;
            existingPlanet.NumberOfMoons = planet.NumberOfMoons;
            existingPlanet.HasAtmosphere = planet.HasAtmosphere;
            existingPlanet.OxygenVolume = planet.OxygenVolume;
            existingPlanet.WaterVolume = planet.WaterVolume;
            existingPlanet.HardnessOfRock = planet.HardnessOfRock;
            existingPlanet.ThreateningCreature = planet.ThreateningCreature;
            existingPlanet.Description = planet.Description;
            existingPlanet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingPlanet;
        }

        public async Task<bool> DeletePlanetAsync(int id, int? userId = null)
        {
            var planet = await _context.Planets.FindAsync(id);
            if (planet == null)
                return false;

            // Check access if user is specified
            if (userId.HasValue && !await UserHasAccessToPlanetAsync(userId.Value, id))
            {
                return false; // User doesn't have access to this planet
            }

            _context.Planets.Remove(planet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserHasAccessToPlanetAsync(int userId, int planetId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // SuperAdmin has access to all planets
            if (user.Role == "superadmin")
                return true;

            // Check if planet is in user's assigned planets
            var assignedPlanetIds = GetUserAssignedPlanetIds(user);
            return assignedPlanetIds.Contains(planetId);
        }

        private List<int> GetUserAssignedPlanetIds(User user)
        {
            if (string.IsNullOrEmpty(user.AssignedPlanetIds))
                return new List<int>();

            try
            {
                return JsonSerializer.Deserialize<List<int>>(user.AssignedPlanetIds) ?? new List<int>();
            }
            catch
            {
                return new List<int>();
            }
        }
    }
}
