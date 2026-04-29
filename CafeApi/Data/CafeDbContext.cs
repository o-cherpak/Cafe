using CafeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeApi.Data;

public class CafeDbContext : DbContext
{
    public CafeDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Customer
        modelBuilder.Entity<Customer>()
            .HasIndex(c => c.Email)
            .IsUnique();

        modelBuilder.Entity<Customer>()
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(140);

        modelBuilder.Entity<Customer>()
            .Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);

        //MenuItem
        modelBuilder.Entity<MenuItem>()
            .Property(m => m.Price)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<MenuItem>()
            .Property(m => m.Name)
            .HasMaxLength(140);

        modelBuilder.Entity<MenuItem>()
            .Property(m => m.Description)
            .HasMaxLength(400);

        modelBuilder.Entity<MenuItem>()
            .Property(m => m.Category)
            .HasConversion<string>()
            .HasMaxLength(100);

        //Order
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId);

        modelBuilder.Entity<Order>()
            .Property(o => o.Status)
            .HasConversion<string>();

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(10,2)");
    }
}