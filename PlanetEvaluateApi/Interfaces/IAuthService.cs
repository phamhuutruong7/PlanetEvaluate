using PlanetEvaluateApi.Models;
using System.Security.Claims;

namespace PlanetEvaluateApi.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(string username, string password);
        Task<User?> GetUserByIdAsync(int userId);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<bool> ValidateUserAsync(string username, string password);
        string GenerateJwtToken(User user);
        int? GetUserIdFromToken(string token);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
