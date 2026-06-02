using CafeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeApi.ModelConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder
            .HasOne(oi => oi.MenuItem)
            .WithMany()
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(10,2)");
    }
}