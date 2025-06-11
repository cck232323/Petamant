namespace MyDotnetApp.Models
{
    public class Activity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Location { get; set; } = string.Empty;
        public int CreatorUserId { get; set; }
        // 如果数据库中没有 CreatedAt 列，请移除或注释掉
        // public DateTime CreatedAt { get; set; }
        
        // 导航属性
        public User CreatorUser { get; set; } = null!;
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}