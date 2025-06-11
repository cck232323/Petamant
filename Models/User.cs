namespace MyDotnetApp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        
        // 导航属性
        public ICollection<Activity> CreatedActivities { get; set; } = new List<Activity>();
        public ICollection<Registration> Registrations { get; set; } = new List<Registration>();
    }
}