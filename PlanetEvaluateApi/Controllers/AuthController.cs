using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanetEvaluateApi.DTOs;
using PlanetEvaluateApi.Interfaces;

namespace PlanetEvaluateApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] LoginUserDto loginDto)
        {
            try
            {
                var result = await _userService.LoginAsync(loginDto);
                _logger.LogInformation("User {Username} logged in successfully", loginDto.Username);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Login attempt failed for user {Username}", loginDto.Username);
                return Unauthorized(new { message = "Invalid username or password" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _userService.LogoutAsync();
                _logger.LogInformation("User logged out successfully");
                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An error occurred during logout" });
            }
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponseDto>> GetMe()
        {
            try
            {
                var result = await _userService.GetCurrentUserAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to user profile");
                return Unauthorized(new { message = "Not authorized" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user profile");
                return StatusCode(500, new { message = "An error occurred while retrieving user profile" });
            }
        }        
        
        [HttpGet("users")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<ActionResult<List<UserResponseDto>>> GetAllUsers()
        {
            try
            {
                var result = await _userService.GetAllUsersAsync();
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to list all users");
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }        
        
        [HttpGet("users/{id}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin")]
        public async Task<ActionResult<UserResponseDto>> GetUserById(int id)
        {
            try
            {
                var result = await _userService.GetUserByIdAsync(id);
                return Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access attempt to user {UserId}", id);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "User {UserId} not found", id);
                return NotFound(new { message = "User not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving user" });
            }
        }        
        
        [HttpPost("users/{userId}/assign-planet/{planetId}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin")]
        public async Task<IActionResult> AssignPlanetToUser(int userId, int planetId, [FromQuery] string permissionLevel = "Read")
        {
            try
            {
                await _userService.AssignPlanetToUserAsync(userId, planetId, permissionLevel);
                _logger.LogInformation("Planet {PlanetId} assigned to user {UserId} with permission {Permission}", planetId, userId, permissionLevel);
                return Ok(new { message = "Planet assigned successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to assign planet {PlanetId} to user {UserId}", planetId, userId);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data for planet assignment: Planet {PlanetId}, User {UserId}", planetId, userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning planet {PlanetId} to user {UserId}", planetId, userId);
                return StatusCode(500, new { message = "An error occurred while assigning planet" });
            }
        }        
        
        [HttpDelete("users/{userId}/remove-planet/{planetId}")]
        [Authorize(Roles = "SuperAdmin,PlanetAdmin")]
        public async Task<IActionResult> RemovePlanetFromUser(int userId, int planetId)
        {
            try
            {
                await _userService.RemovePlanetFromUserAsync(userId, planetId);
                _logger.LogInformation("Planet {PlanetId} removed from user {UserId}", planetId, userId);
                return Ok(new { message = "Planet removed successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized attempt to remove planet {PlanetId} from user {UserId}", planetId, userId);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid data for planet removal: Planet {PlanetId}, User {UserId}", planetId, userId);
                return BadRequest(new { message = ex.Message });
            }            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing planet {PlanetId} from user {UserId}", planetId, userId);
                return StatusCode(500, new { message = "An error occurred while removing planet" });
            }
        }
    }
}
