using System.ComponentModel.DataAnnotations;

namespace MyDotnetApp.DTOs
{
    public class UserRegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string UserName { get; set; } = string.Empty;
        
        [Required]
        [MinLength(6)]
        public string Password { get; set; } = string.Empty;
    }
}