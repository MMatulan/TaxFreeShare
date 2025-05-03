using Microsoft.EntityFrameworkCore;
using TaxFreeShareAPI.Models;

namespace TaxFreeShareAPI.Data;

public class TaxFreeDbContext : DbContext
{
    public TaxFreeDbContext(DbContextOptions<TaxFreeDbContext> options) : base(options) { }
    
    public TaxFreeDbContext() { }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    
    public DbSet<Order> Orders { get; set; }
    
    public DbSet<OrderItem> OrderItems { get; set; }
    
    public DbSet<Transaction> Transactions { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Eksempel på seed data
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Lindt Milk Chocolate", Category = "Chocolate", Price = 49.99m, Brand = "Lindt" },
            new Product { Id = 2, Name = "Dior Sauvage", Category = "Perfume", Price = 1199.99m, Brand = "Dior" }
        );

        modelBuilder.Entity<User>().HasData(
            new User { Id = 1, Name = "Admin", Email = "admin@taxfreeshare.com", PasswordHash = "hashedpassword123", Role = "Admin", CreatedAt = DateTime.UtcNow }
        );
        
        modelBuilder.Entity<Order>().HasData(
            new Order { Id = 1, UserId = 1, OrderDate = DateTime.UtcNow, TotalAmount = 49.99m, Status = "Completed" }
        );
    }
}