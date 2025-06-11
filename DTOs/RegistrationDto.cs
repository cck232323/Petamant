namespace MyDotnetApp.DTOs
{
    public class RegistrationDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty; // 添加此属性
        public UserDto User { get; set; } = null!; // 添加此属性
        public string PetInfo { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
    }

    public class RegistrationCreateDto
    {
        public int ActivityId { get; set; }
        public string PetInfo { get; set; } = string.Empty;
    }
}