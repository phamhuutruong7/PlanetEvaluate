using PlanetEvaluateApi.Models;

namespace PlanetEvaluateApi.Interfaces
{
    public interface IPlanetService
    {
        Task<IEnumerable<Planet>> GetAllPlanetsAsync(int? userId = null);
        Task<Planet?> GetPlanetByIdAsync(int id, int? userId = null);
        Task<Planet> CreatePlanetAsync(Planet planet);
        Task<Planet?> UpdatePlanetAsync(int id, Planet planet, int? userId = null);
        Task<bool> DeletePlanetAsync(int id, int? userId = null);
        Task<bool> UserHasAccessToPlanetAsync(int userId, int planetId);
    }
}
