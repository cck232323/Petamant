namespace MyDotnetApp.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty; // 添加此属性
        public DateTime CreatedAt { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class CommentCreateDto
    {
        public string Content { get; set; } = string.Empty;
        public int ActivityId { get; set; }
        public int UserId { get; set; } // 添加此属性
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // 添加此属性
        public string UserName { get; set; } = string.Empty;// 添加此属性
    }
}