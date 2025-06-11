using Microsoft.EntityFrameworkCore;
using MyDotnetApp.Models;

namespace MyDotnetApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Activity>(entity =>
            {
                // 确保没有配置 CreatedAt 属性
                // entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });
            
            // 配置 User 和 Activity 之间的一对多关系（创建者关系）
            modelBuilder.Entity<Activity>()
                .HasOne(a => a.CreatorUser)
                .WithMany(u => u.CreatedActivities)
                .HasForeignKey(a => a.CreatorUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 配置 User 和 Activity 之间的多对多关系（通过 Registration）
            modelBuilder.Entity<Registration>()
                .HasKey(r => new { r.UserId, r.ActivityId });

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.User)
                .WithMany(u => u.Registrations)
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<Registration>()
                .HasOne(r => r.Activity)
                .WithMany(a => a.Registrations)
                .HasForeignKey(r => r.ActivityId);

            // 配置 Comment 关系
            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Activity)
                .WithMany(a => a.Comments)
                .HasForeignKey(c => c.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}