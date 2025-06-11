namespace MyDotnetApp.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
    }

    public class UserRegisterDto
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UserLoginDto
    {
        public required string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public required string Password { get; set; }
    }

    public class UserLoginResponseDto
    {
        public int Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Token { get; set; }
    }
}