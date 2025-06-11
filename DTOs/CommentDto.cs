namespace MyDotnetApp.DTOs
{
    public class CommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public UserDto User { get; set; } = null!;
    }

    public class CommentCreateDto
    {
        public string Content { get; set; } = string.Empty;
        public int ActivityId { get; set; }
    }
}