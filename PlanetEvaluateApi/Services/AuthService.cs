using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlanetEvaluateApi.Data;
using PlanetEvaluateApi.Interfaces;
using PlanetEvaluateApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace PlanetEvaluateApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly PlanetEvaluateDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(PlanetEvaluateDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Update last login
            user.LastLogin = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await GetUserByUsernameAsync(username);
            return user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        }        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = jwtSettings["Key"] ?? "YourSuperSecretKeyThatIsAtLeast32CharactersLong!";
            var issuer = jwtSettings["Issuer"] ?? "PlanetEvaluateApi";
            var audience = jwtSettings["Audience"] ?? "PlanetEvaluateClient";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("AssignedPlanetIds", user.AssignedPlanetIds ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24), // Token expires in 24 hours
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int? GetUserIdFromToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token.Replace("Bearer ", ""));
                
                var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            catch
            {
                // Token parsing failed
            }
            
            return null;
        }
    }
}
