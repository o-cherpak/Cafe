using CafeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeApi.ModelConfigurations;

public class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder
            .Property(m => m.Price)
            .HasColumnType("decimal(10,2)");

        builder
            .Property(m => m.Name)
            .HasMaxLength(140);

        builder
            .Property(m => m.Description)
            .HasMaxLength(400);

        builder
            .Property(m => m.Category)
            .HasConversion<string>()
            .HasMaxLength(100);
    }
}