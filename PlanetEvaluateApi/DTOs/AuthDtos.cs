using System.ComponentModel.DataAnnotations;

namespace PlanetEvaluateApi.DTOs
{
    public class LoginUserDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class UserResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<int> AssignedPlanetIds { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? Token { get; set; }
    }
}
