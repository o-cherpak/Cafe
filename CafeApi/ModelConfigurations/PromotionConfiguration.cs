using CafeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeApi.ModelConfigurations;

public class PromotionConfiguration : IEntityTypeConfiguration<Promotion>
{
    public void Configure(EntityTypeBuilder<Promotion> builder)
    {
        builder
            .Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(p => p.DiscountValue)
            .HasColumnType("decimal(10,2)");

        builder
            .Property(p => p.Description)
            .HasMaxLength(400);

        builder
            .Property(p => p.DiscountType)
            .HasConversion<string>();
    }
}