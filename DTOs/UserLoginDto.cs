using System.ComponentModel.DataAnnotations;

namespace MyDotnetApp.DTOs
{
    public class UserLoginRequestDto
    {
        [Required]
        public string? UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}