using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using System.Security.Claims;

namespace PlanetEvaluateApi.Services
{
    /// <summary>
    /// Service responsible for planet business logic operations
    /// </summary>
    public class PlanetBusinessService : IPlanetBusinessService
    {
        private readonly IPlanetService _planetService;
        private readonly IPlanetMappingService _mappingService;
        private readonly ILogger<PlanetBusinessService> _logger;

        public PlanetBusinessService(
            IPlanetService planetService,
            IPlanetMappingService mappingService,
            ILogger<PlanetBusinessService> logger)
        {
            _planetService = planetService;
            _mappingService = mappingService;
            _logger = logger;
        }

        /// <summary>
        /// Gets all accessible planets for a user and returns them as DTOs
        /// </summary>
        /// <param name="userId">The ID of the user requesting planets</param>
        /// <returns>Collection of planet DTOs accessible to the user</returns>
        public async Task<IEnumerable<PlanetDto>> GetAllPlanetsAsync(int userId)
        {
            var planets = await _planetService.GetAllPlanetsAsync(userId);
            return _mappingService.MapToPlanetDtos(planets);
        }

        /// <summary>
        /// Gets a specific planet by ID for a user and returns it as a DTO
        /// </summary>
        /// <param name="planetId">The ID of the planet to retrieve</param>
        /// <param name="userId">The ID of the user requesting the planet</param>
        /// <returns>Planet DTO or null if not found/accessible</returns>
        public async Task<PlanetDto?> GetPlanetByIdAsync(int planetId, int userId)
        {
            var planet = await _planetService.GetPlanetByIdAsync(planetId, userId);
            return planet != null ? _mappingService.MapToPlanetDto(planet) : null;
        }

        /// <summary>
        /// Creates a new planet from a CreatePlanetDto for a specific user
        /// </summary>
        /// <param name="createDto">The DTO containing planet creation data</param>
        /// <param name="userId">The ID of the user creating the planet</param>
        /// <returns>The created planet as a DTO</returns>
        public async Task<PlanetDto> CreatePlanetAsync(CreatePlanetDto createDto, int userId)
        {
            var planet = _mappingService.MapToEntity(createDto);
            var createdPlanet = await _planetService.CreatePlanetAsync(planet, userId);
            return _mappingService.MapToPlanetDto(createdPlanet);
        }

        /// <summary>
        /// Updates an existing planet using an UpdatePlanetDto for a specific user
        /// </summary>
        /// <param name="planetId">The ID of the planet to update</param>
        /// <param name="updateDto">The DTO containing update data</param>
        /// <param name="userId">The ID of the user updating the planet</param>
        /// <returns>The updated planet as a DTO or null if not found/accessible</returns>
        public async Task<PlanetDto?> UpdatePlanetAsync(int planetId, UpdatePlanetDto updateDto, int userId)
        {
            // Get existing planet to preserve unchanged fields
            var existingPlanet = await _planetService.GetPlanetByIdAsync(planetId, userId);
            if (existingPlanet == null)
            {
                return null;
            }

            // Merge the update DTO with the existing planet
            var updatedPlanet = _mappingService.MergeUpdateDto(existingPlanet, updateDto);

            // Perform the update
            var result = await _planetService.UpdatePlanetAsync(planetId, updatedPlanet, userId);
            return result != null ? _mappingService.MapToPlanetDto(result) : null;
        }

        /// <summary>
        /// Deletes a planet for a specific user
        /// </summary>
        /// <param name="planetId">The ID of the planet to delete</param>
        /// <param name="userId">The ID of the user deleting the planet</param>
        /// <returns>True if deleted successfully, false if not found/accessible</returns>
        public async Task<bool> DeletePlanetAsync(int planetId, int userId)
        {
            return await _planetService.DeletePlanetAsync(planetId, userId);
        }

        /// <summary>
        /// Extracts the current user ID from JWT claims
        /// </summary>
        /// <param name="principal">The claims principal containing user information</param>
        /// <returns>The user ID or null if not found/invalid</returns>
        public int? GetCurrentUserId(ClaimsPrincipal principal)
        {
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            return null;
        }
    }
}
