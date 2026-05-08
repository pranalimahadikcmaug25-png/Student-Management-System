using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ─── Student configuration ────────────────────────────────────────
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Name).IsRequired().HasMaxLength(100);
                entity.Property(s => s.Email).IsRequired().HasMaxLength(150);
                entity.HasIndex(s => s.Email).IsUnique();
                entity.Property(s => s.Course).IsRequired().HasMaxLength(100);
                // CreatedDate is set in code (DateTime.UtcNow) — no DB default needed
                entity.Property(s => s.CreatedDate).IsRequired();
            });

            // ─── User configuration ───────────────────────────────────────────
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Username).IsRequired().HasMaxLength(100);
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.Role).IsRequired().HasMaxLength(50).HasDefaultValue("User");
                // CreatedDate is set in code (DateTime.UtcNow) — no DB default needed
                entity.Property(u => u.CreatedDate).IsRequired();
            });
        }
    }
}
