using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Data;
using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using System.Security.Claims;
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

        public async Task<IEnumerable<Planet>> GetAllPlanetsAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            var accessiblePlanetIds = await GetUserAccessiblePlanetIdsAsync(userId);
            
            if (!accessiblePlanetIds.Any())
                return new List<Planet>();

            return await _context.Planets
                .Where(p => accessiblePlanetIds.Contains(p.Id))
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<Planet?> GetPlanetByIdAsync(int id, int userId)
        {
            if (!await CanUserAccessPlanetAsync(userId, id))
                return null;

            return await _context.Planets.FindAsync(id);
        }

        public async Task<Planet> CreatePlanetAsync(Planet planet, int userId)
        {
            if (!await CanUserCreatePlanetAsync(userId))
                throw new UnauthorizedAccessException("User does not have permission to create planets");

            planet.CreatedAt = DateTime.UtcNow;
            planet.UpdatedAt = DateTime.UtcNow;
            
            _context.Planets.Add(planet);
            await _context.SaveChangesAsync();
            
            return planet;
        }

        public async Task<Planet?> UpdatePlanetAsync(int id, Planet planet, int userId)
        {
            if (!await CanUserEditPlanetAsync(userId, id))
                return null;

            var existingPlanet = await _context.Planets.FindAsync(id);
            if (existingPlanet == null)
                return null;

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

        public async Task<bool> DeletePlanetAsync(int id, int userId)
        {
            if (!await CanUserDeletePlanetAsync(userId, id))
                return false;

            var planet = await _context.Planets.FindAsync(id);
            if (planet == null)
                return false;

            _context.Planets.Remove(planet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanUserAccessPlanetAsync(int userId, int planetId)
        {
            var user = await GetUserAsync(userId);
            if (user == null) return false;

            return user.Role.ToLower() switch
            {
                "superadmin" => true,
                "planetadmin" => await IsUserAssignedToPlanetAsync(user, planetId),
                "viewer1" => planetId == 1, // Only planet 1
                "viewer2" => planetId == 1 || planetId == 3, // Only planets 1 and 3
                "viewer" => await IsUserAssignedToPlanetAsync(user, planetId), // Generic viewer - use assigned planets
                _ => false
            };
        }

        public async Task<bool> CanUserCreatePlanetAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            if (user == null) return false;

            return user.Role.ToLower() switch
            {
                "superadmin" => true,
                "planetadmin" => true,
                _ => false // Viewers cannot create planets
            };
        }

        public async Task<bool> CanUserEditPlanetAsync(int userId, int planetId)
        {
            var user = await GetUserAsync(userId);
            if (user == null) return false;

            return user.Role.ToLower() switch
            {
                "superadmin" => true,
                "planetadmin" => await IsUserAssignedToPlanetAsync(user, planetId),
                _ => false // Viewers cannot edit planets
            };
        }

        public async Task<bool> CanUserDeletePlanetAsync(int userId, int planetId)
        {
            var user = await GetUserAsync(userId);
            if (user == null) return false;

            return user.Role.ToLower() switch
            {
                "superadmin" => true,
                "planetadmin" => await IsUserAssignedToPlanetAsync(user, planetId),
                _ => false // Viewers cannot delete planets
            };
        }

        public async Task<List<int>> GetUserAccessiblePlanetIdsAsync(int userId)
        {
            var user = await GetUserAsync(userId);
            if (user == null) return new List<int>();

            return user.Role.ToLower() switch
            {
                "superadmin" => await _context.Planets.Select(p => p.Id).ToListAsync(),
                "planetadmin" => GetUserAssignedPlanetIds(user),
                "viewer1" => new List<int> { 1 }, // Only planet 1
                "viewer2" => new List<int> { 1, 3 }, // Only planets 1 and 3
                "viewer" => GetUserAssignedPlanetIds(user), // Generic viewer - use assigned planets
                _ => new List<int>()
            };
        }

        private async Task<User?> GetUserAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }        private Task<bool> IsUserAssignedToPlanetAsync(User user, int planetId)
        {
            var assignedPlanetIds = GetUserAssignedPlanetIds(user);
            return Task.FromResult(assignedPlanetIds.Contains(planetId));
        }        private List<int> GetUserAssignedPlanetIds(User user)
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

        #region DTO Operations and Mapping

        /// <summary>
        /// Gets all accessible planets for a user and returns them as DTOs
        /// </summary>
        public async Task<IEnumerable<PlanetDto>> GetAllPlanetDtosAsync(int userId)
        {
            var planets = await GetAllPlanetsAsync(userId);
            return planets.Select(MapToPlanetDto);
        }

        /// <summary>
        /// Gets a specific planet by ID for a user and returns it as a DTO
        /// </summary>
        public async Task<PlanetDto?> GetPlanetDtoByIdAsync(int id, int userId)
        {
            var planet = await GetPlanetByIdAsync(id, userId);
            return planet != null ? MapToPlanetDto(planet) : null;
        }

        /// <summary>
        /// Creates a new planet from a CreatePlanetDto for a specific user
        /// </summary>
        public async Task<PlanetDto> CreatePlanetFromDtoAsync(CreatePlanetDto createDto, int userId)
        {
            var planet = MapToEntity(createDto);
            var createdPlanet = await CreatePlanetAsync(planet, userId);
            return MapToPlanetDto(createdPlanet);
        }

        /// <summary>
        /// Updates an existing planet using an UpdatePlanetDto for a specific user
        /// </summary>
        public async Task<PlanetDto?> UpdatePlanetFromDtoAsync(int id, UpdatePlanetDto updateDto, int userId)
        {
            // Get existing planet to preserve unchanged fields
            var existingPlanet = await GetPlanetByIdAsync(id, userId);
            if (existingPlanet == null)
            {
                return null;
            }

            // Merge the update DTO with the existing planet
            var updatedPlanet = MergeUpdateDto(existingPlanet, updateDto);

            // Perform the update
            var result = await UpdatePlanetAsync(id, updatedPlanet, userId);
            return result != null ? MapToPlanetDto(result) : null;
        }

        /// <summary>
        /// Extracts the current user ID from JWT claims
        /// </summary>
        public int? GetCurrentUserId(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }

        #endregion

        #region DTO Mapping Methods

        /// <summary>
        /// Maps a Planet entity to a PlanetDto
        /// </summary>
        private PlanetDto MapToPlanetDto(Planet planet)
        {
            return new PlanetDto
            {
                Id = planet.Id,
                Name = planet.Name,
                Type = planet.Type,
                Mass = planet.Mass,
                Radius = planet.Radius,
                DistanceFromSun = planet.DistanceFromSun,
                NumberOfMoons = planet.NumberOfMoons,
                HasAtmosphere = planet.HasAtmosphere,
                OxygenVolume = planet.OxygenVolume,
                WaterVolume = planet.WaterVolume,
                HardnessOfRock = planet.HardnessOfRock,
                ThreateningCreature = planet.ThreateningCreature,
                CreatedAt = planet.CreatedAt,
                UpdatedAt = planet.UpdatedAt,
                Description = planet.Description
            };
        }

        /// <summary>
        /// Maps a CreatePlanetDto to a Planet entity
        /// </summary>
        private Planet MapToEntity(CreatePlanetDto createDto)
        {
            return new Planet
            {
                Name = createDto.Name,
                Type = createDto.Type,
                Mass = createDto.Mass,
                Radius = createDto.Radius,
                DistanceFromSun = createDto.DistanceFromSun,
                NumberOfMoons = createDto.NumberOfMoons,
                HasAtmosphere = createDto.HasAtmosphere,
                OxygenVolume = createDto.OxygenVolume,
                WaterVolume = createDto.WaterVolume,
                HardnessOfRock = createDto.HardnessOfRock,
                ThreateningCreature = createDto.ThreateningCreature,
                Description = createDto.Description
            };
        }

        /// <summary>
        /// Merges an UpdatePlanetDto with an existing Planet entity, preserving unchanged fields
        /// </summary>
        private Planet MergeUpdateDto(Planet existingPlanet, UpdatePlanetDto updateDto)
        {
            return new Planet
            {
                Id = existingPlanet.Id,
                Name = updateDto.Name ?? existingPlanet.Name,
                Type = updateDto.Type ?? existingPlanet.Type,
                Mass = updateDto.Mass ?? existingPlanet.Mass,
                Radius = updateDto.Radius ?? existingPlanet.Radius,
                DistanceFromSun = updateDto.DistanceFromSun ?? existingPlanet.DistanceFromSun,
                NumberOfMoons = updateDto.NumberOfMoons ?? existingPlanet.NumberOfMoons,
                HasAtmosphere = updateDto.HasAtmosphere ?? existingPlanet.HasAtmosphere,
                OxygenVolume = updateDto.OxygenVolume ?? existingPlanet.OxygenVolume,
                WaterVolume = updateDto.WaterVolume ?? existingPlanet.WaterVolume,
                HardnessOfRock = updateDto.HardnessOfRock ?? existingPlanet.HardnessOfRock,
                ThreateningCreature = updateDto.ThreateningCreature ?? existingPlanet.ThreateningCreature,
                Description = updateDto.Description ?? existingPlanet.Description,
                CreatedAt = existingPlanet.CreatedAt
            };
        }

        #endregion
    }
}
