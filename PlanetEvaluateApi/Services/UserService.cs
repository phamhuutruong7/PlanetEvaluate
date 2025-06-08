using Microsoft.EntityFrameworkCore;
using PlanetEvaluateApi.Data;
using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using System.Security.Claims;
using System.Text.Json;

namespace PlanetEvaluateApi.Services
{
    public class UserService : IUserService
    {
        private readonly PlanetEvaluateDbContext _context;
        private readonly IAuthService _authService;
        private readonly IPlanetService _planetService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;

        public UserService(
            PlanetEvaluateDbContext context,
            IAuthService authService,
            IPlanetService planetService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<UserService> logger)
        {
            _context = context;
            _authService = authService;
            _planetService = planetService;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<UserResponseDto> LoginAsync(LoginUserDto loginDto)
        {
            var token = await _authService.LoginAsync(loginDto.Username, loginDto.Password);
            var user = await _authService.GetUserByUsernameAsync(loginDto.Username);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            return MapToUserResponseDto(user, token);
        }

        public Task LogoutAsync()
        {
            // In a stateless JWT implementation, logout is typically handled client-side
            // You might want to implement token blacklisting here if needed
            return Task.CompletedTask;
        }

        public async Task<UserResponseDto> GetCurrentUserAsync()
        {
            var userId = await GetCurrentUserIdAsync();
            var user = await _authService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            return MapToUserResponseDto(user);
        }

        public async Task<List<UserResponseDto>> GetAllUsersAsync()
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var currentUser = await _authService.GetUserByIdAsync(currentUserId);
            
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            // Only SuperAdmin can view all users
            if (currentUser.Role.ToLower() != "superadmin")
            {
                throw new UnauthorizedAccessException("Insufficient permissions to view all users");
            }

            var users = await _context.Users.ToListAsync();
            return users.Select(u => MapToUserResponseDto(u)).ToList();
        }

        public async Task<UserResponseDto> GetUserByIdAsync(int id)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var currentUser = await _authService.GetUserByIdAsync(currentUserId);
            
            if (currentUser == null)
            {
                throw new UnauthorizedAccessException("User not found");
            }

            // Users can view their own profile, SuperAdmin can view any profile
            if (currentUserId != id && currentUser.Role.ToLower() != "superadmin")
            {
                throw new UnauthorizedAccessException("Insufficient permissions to view this user");
            }

            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            return MapToUserResponseDto(user);        }

        public async Task AssignPlanetToUserAsync(int userId, int planetId, string permissionLevel = "Read")
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var currentUser = await _authService.GetUserByIdAsync(currentUserId);
            
            if (currentUser == null || currentUser.Role.ToLower() != "superadmin")
            {
                throw new UnauthorizedAccessException("Only SuperAdmin can assign planets to users");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Get current assigned planets
            var assignedPlanetIds = GetUserAssignedPlanetIds(user);
            
            // Add the new planet if not already assigned
            if (!assignedPlanetIds.Contains(planetId))
            {
                assignedPlanetIds.Add(planetId);
                user.AssignedPlanetIds = JsonSerializer.Serialize(assignedPlanetIds);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemovePlanetFromUserAsync(int userId, int planetId)
        {
            var currentUserId = await GetCurrentUserIdAsync();
            var currentUser = await _authService.GetUserByIdAsync(currentUserId);
            
            if (currentUser == null || currentUser.Role.ToLower() != "superadmin")
            {
                throw new UnauthorizedAccessException("Only SuperAdmin can remove planet assignments");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            // Get current assigned planets
            var assignedPlanetIds = GetUserAssignedPlanetIds(user);
            
            // Remove the planet if assigned
            if (assignedPlanetIds.Contains(planetId))
            {
                assignedPlanetIds.Remove(planetId);
                user.AssignedPlanetIds = JsonSerializer.Serialize(assignedPlanetIds);
                await _context.SaveChangesAsync();
            }        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _authService.GetUserByIdAsync(id);
        }        public Task<int> GetCurrentUserIdAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }

            var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                throw new UnauthorizedAccessException("Invalid user token");
            }

            return Task.FromResult(userId);
        }

        private UserResponseDto MapToUserResponseDto(User user, string? token = null)
        {
            return new UserResponseDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = MapRoleToFrontend(user.Role),
                AssignedPlanetIds = GetUserAssignedPlanetIds(user),
                CreatedAt = user.CreatedAt,
                LastLogin = user.LastLogin,
                Token = token
            };
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

        private string MapRoleToFrontend(string backendRole)
        {
            return backendRole.ToLower() switch
            {
                "superadmin" => "SuperAdmin",
                "planetadmin" => "PlanetAdmin",
                "viewer1" => "ViewerType1",
                "viewer2" => "ViewerType2",
                "viewer" => "Viewer",
                _ => "Viewer"
            };
        }
    }
}
