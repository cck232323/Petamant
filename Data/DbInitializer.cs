using MyDotnetApp.Models;

namespace MyDotnetApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // 确保数据库已创建
            context.Database.EnsureCreated();
            
            // 检查是否已有用户
            if (context.Users.Any())
            {
                return; // 数据库已经有数据
            }

            // 添加示例用户
            var users = new User[]
            {
                new User { Email = "user1@example.com", UserName = "User1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") },
                new User { Email = "user2@example.com", UserName = "User2", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123") }
            };

            context.Users.AddRange(users);
            context.SaveChanges();

            // 添加示例活动
            var activities = new Activity[]
            {
                new Activity {
                    Title = "狗狗公园聚会",
                    Description = "带上您的狗狗来参加这个有趣的聚会！",
                    // Date = DateTime.Now.AddDays(7),
                    Date = DateTime.UtcNow.AddDays(7),
                    Location = "中央公园",
                    CreatorUserId = users[0].Id
                },
                new Activity {
                    Title = "猫咪咖啡日",
                    Description = "与其他猫咪爱好者一起享受下午茶时光。",
                    Date = DateTime.UtcNow.AddDays(14),
                    Location = "宠物友好咖啡馆",
                    CreatorUserId = users[1].Id
                }
            };

            context.Activities.AddRange(activities);
            context.SaveChanges();
        }
    }
}