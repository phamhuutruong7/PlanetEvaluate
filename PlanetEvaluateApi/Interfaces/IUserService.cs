using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Models;
using System.Security.Claims;

namespace PlanetEvaluateApi.Interfaces
{
    public interface IUserService
    {
        // Authentication
        Task<UserResponseDto> LoginAsync(LoginUserDto loginDto);
        Task LogoutAsync();

        // User management
        Task<UserResponseDto> GetCurrentUserAsync();
        Task<List<UserResponseDto>> GetAllUsersAsync();
        Task<UserResponseDto> GetUserByIdAsync(int id);
          
          // Planet assignments
        Task AssignPlanetToUserAsync(int userId, int planetId, string permissionLevel = "Read");
        Task RemovePlanetFromUserAsync(int userId, int planetId);
        
        // Utility methods
        Task<User?> GetUserEntityByIdAsync(int id);
        Task<int> GetCurrentUserIdAsync();
    }
}
