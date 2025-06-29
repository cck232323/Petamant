namespace MyDotnetApp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string UserName { get; set; } = string.Empty;
        
        // 添加父评论ID，可为空表示顶层评论
        public int? ParentCommentId { get; set; }
        
        // 添加回复目标用户ID和名称
        public int? ReplyToUserId { get; set; }
        public string? ReplyToUserName { get; set; }
        
        // 导航属性
        public User User { get; set; } = null!;
        public Activity Activity { get; set; } = null!;
        
        // 添加父评论导航属性
        public Comment? ParentComment { get; set; }
        
        // 添加子评论集合
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}