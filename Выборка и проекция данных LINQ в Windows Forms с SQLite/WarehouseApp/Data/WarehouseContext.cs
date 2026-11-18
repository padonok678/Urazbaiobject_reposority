using Microsoft.EntityFrameworkCore;
using WarehouseApp.Models;

namespace WarehouseApp.Data;

public class WarehouseContext : DbContext
{
    
    public static string DbPath => Path.Combine(AppContext.BaseDirectory, "warehouse.db");

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Category>().ToTable("Categories");
        mb.Entity<Product>().ToTable("Products");

        mb.Entity<Product>()
          .HasOne(p => p.Category)
          .WithMany(c => c.Products)
          .HasForeignKey(p => p.CategoryId);
    }
}
