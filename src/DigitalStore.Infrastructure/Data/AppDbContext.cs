using DigitalStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DigitalStore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── User ──────────────────────────────────────────────
        modelBuilder.Entity<User>(e =>
        {
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.PasswordHash).HasMaxLength(512);
            e.Property(u => u.Role).HasMaxLength(50).HasDefaultValue("User");
        });

        // ── Product ───────────────────────────────────────────
        modelBuilder.Entity<Product>(e =>
        {
            e.Property(p => p.Price).HasColumnType("decimal(18,2)");
            e.Property(p => p.Type).HasConversion<string>();
        });

        // ── Order ─────────────────────────────────────────────
        modelBuilder.Entity<Order>(e =>
        {
            e.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(o => o.Status).HasConversion<string>();
        });

        // ── OrderDetail ───────────────────────────────────────
        modelBuilder.Entity<OrderDetail>(e =>
        {
            e.Property(o => o.PriceAtPurchase).HasColumnType("decimal(18,2)");
        });

        // ── UserAccess — unique constraint ────────────────────
        modelBuilder.Entity<UserAccess>(e =>
        {
            e.HasIndex(ua => new { ua.UserId, ua.ProductId }).IsUnique();
        });

        // ── Seed data: danh mục mặc định ─────────────────────
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "AutoCAD Add-ins", Slug = "autocad-addins" },
            new Category { Id = 2, Name = "Revit Scripts", Slug = "revit-scripts" },
            new Category { Id = 3, Name = "Khóa học Video", Slug = "video-courses" }
        );
    }
}
