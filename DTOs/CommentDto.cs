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
        
        // 添加父评论ID
        public int? ParentCommentId { get; set; }
        
        // 添加回复目标用户信息
        public int? ReplyToUserId { get; set; }
        public string? ReplyToUserName { get; set; }
        
        // 添加子评论集合
        public ICollection<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }

    public class CommentCreateDto
    {
        public string Content { get; set; } = string.Empty;
        public int ActivityId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string UserName { get; set; } = string.Empty;
        
        // 添加父评论ID，可为空
        public int? ParentCommentId { get; set; }
        
        // 添加回复目标用户信息
        public int? ReplyToUserId { get; set; }
        public string? ReplyToUserName { get; set; }
    }
}