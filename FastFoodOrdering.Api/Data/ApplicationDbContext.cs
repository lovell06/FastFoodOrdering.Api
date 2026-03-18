using FastFoodOrdering.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace FastFoodOrdering.Api.Data;

public class ApplicationDbContext : DbContext
{
//     private readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
//     {
//         builder.AddFilter(DbLoggerCategory.Query.Name, LogLevel.Information);
//         builder.AddConsole();
//     });

    private readonly IConfiguration _config;

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration config) : base(options)
    {
        _config = config;
    }

    // Khai báo các bảng trong Database
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        var connString = _config.GetConnectionString("DefaultConnection");
        // optionsBuilder.UseLoggerFactory(_loggerFactory);
        optionsBuilder.UseSqlServer(connString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 1. Cấu hình kiểu Decimal cho tiền tệ (Tránh lỗi thất thoát dữ liệu)
        modelBuilder.Entity<Product>().Property(p => p.Price).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderDetail>().Property(od => od.UnitPrice).HasColumnType("decimal(18,2)");
        modelBuilder.Entity<OrderDetail>().Property(od => od.SubTotal).HasColumnType("decimal(18,2)");

        // 2. Cấu hình độ dài chuỗi (MaxLength) cho từng model

        // Cấu hình cho bảng User
        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.FullName).HasMaxLength(100);
            entity.Property(u => u.Email).HasMaxLength(100);
            entity.Property(u => u.Password).HasMaxLength(100);
            entity.Property(u => u.Phone).HasMaxLength(100);
        });

        // Cấu hình cho bảng Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Name).HasMaxLength(100);
            entity.Property(p => p.Description).HasMaxLength(1000);
            entity.Property(p => p.ImageUrl).HasMaxLength(100);
        });

        // 3. Cấu hình các mối quan hệ (Khóa ngoại)

        // User (1) - Order (N)
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);

        // Order (1) - OrderDetail (N)
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderId);

        // Product (1) - OrderDetail (N)
        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductId);
    }
}
