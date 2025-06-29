namespace MyDotnetApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string UserName { get; set; } = string.Empty; // 添加此属性

        
        // 导航属性
        public User User { get; set; } = null!;
        public Activity Activity { get; set; } = null!;
    }
}