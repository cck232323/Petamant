using System;

namespace MyDotnetApp.Models
{
    public class Registration
    {
        // 这里是您的 Registration 模型类定义
        // 请确保它与上下文配置一致
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ActivityId { get; set; }
        public string PetInfo { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // 导航属性
        public User User { get; set; } = null!;
        public Activity Activity { get; set; } = null!;
    }
}