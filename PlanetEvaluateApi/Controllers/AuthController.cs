using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Interfaces;
using System.Security.Claims;
using System.Text.Json;

namespace PlanetEvaluateApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var token = await _authService.LoginAsync(request.Username, request.Password);
                var user = await _authService.GetUserByUsernameAsync(request.Username);
                
                if (user == null)
                {
                    return Unauthorized(new { message = "Invalid credentials" });
                }

                // Parse assigned planet IDs
                int? assignedPlanetId = null;
                string? assignedPlanetName = null;
                
                if (!string.IsNullOrEmpty(user.AssignedPlanetIds))
                {
                    try
                    {
                        var planetIds = JsonSerializer.Deserialize<List<int>>(user.AssignedPlanetIds);
                        if (planetIds?.Any() == true)
                        {
                            assignedPlanetId = planetIds.First(); // For frontend compatibility, use first planet
                            // You could fetch planet name here if needed
                        }
                    }
                    catch
                    {
                        // Invalid JSON, ignore
                    }
                }

                var response = new LoginResponseDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = MapRoleToFrontend(user.Role),
                    AssignedPlanetId = assignedPlanetId,
                    AssignedPlanetName = assignedPlanetName,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin,
                    Token = token
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var user = await _authService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Parse assigned planet IDs
                int? assignedPlanetId = null;
                string? assignedPlanetName = null;
                
                if (!string.IsNullOrEmpty(user.AssignedPlanetIds))
                {
                    try
                    {
                        var planetIds = JsonSerializer.Deserialize<List<int>>(user.AssignedPlanetIds);
                        if (planetIds?.Any() == true)
                        {
                            assignedPlanetId = planetIds.First(); // For frontend compatibility, use first planet
                        }
                    }
                    catch
                    {
                        // Invalid JSON, ignore
                    }
                }

                var response = new UserDto
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = MapRoleToFrontend(user.Role),
                    AssignedPlanetId = assignedPlanetId,
                    AssignedPlanetName = assignedPlanetName,
                    CreatedAt = user.CreatedAt,
                    LastLogin = user.LastLogin
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }

        private string MapRoleToFrontend(string backendRole)
        {
            return backendRole.ToLower() switch
            {
                "superadmin" => "SuperAdmin",
                "planetadmin" => "PlanetAdmin",
                "viewer" => "ViewerType1", // Default viewer type
                _ => "ViewerType1"
            };
        }
    }
}
