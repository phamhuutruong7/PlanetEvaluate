using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Models;
using System.Security.Claims;

namespace PlanetEvaluateApi.Interfaces
{
    public interface IPlanetService
    {
        // Entity-level operations
        Task<IEnumerable<Planet>> GetAllPlanetsAsync(int userId);
        Task<Planet?> GetPlanetByIdAsync(int id, int userId);
        Task<Planet> CreatePlanetAsync(Planet planet, int userId);
        Task<Planet?> UpdatePlanetAsync(int id, Planet planet, int userId);
        Task<bool> DeletePlanetAsync(int id, int userId);
        Task<bool> CanUserAccessPlanetAsync(int userId, int planetId);
        Task<bool> CanUserCreatePlanetAsync(int userId);
        Task<bool> CanUserEditPlanetAsync(int userId, int planetId);
        Task<bool> CanUserDeletePlanetAsync(int userId, int planetId);
        Task<List<int>> GetUserAccessiblePlanetIdsAsync(int userId);

        // Business logic operations with DTOs
        Task<IEnumerable<PlanetDto>> GetAllPlanetDtosAsync(int userId);
        Task<PlanetDto?> GetPlanetDtoByIdAsync(int id, int userId);
        Task<PlanetDto> CreatePlanetFromDtoAsync(CreatePlanetDto createDto, int userId);
        Task<PlanetDto?> UpdatePlanetFromDtoAsync(int id, UpdatePlanetDto updateDto, int userId);
        int? GetCurrentUserId(ClaimsPrincipal principal);
    }
}
