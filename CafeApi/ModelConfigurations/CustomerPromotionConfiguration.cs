using CafeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeApi.ModelConfigurations;

public class CustomerPromotionConfiguration : IEntityTypeConfiguration<CustomerPromotion>
{
    public void Configure(EntityTypeBuilder<CustomerPromotion> builder)
    {
        builder
            .HasOne(cp => cp.Customer)
            .WithMany(c => c.Promotions)
            .HasForeignKey(cp => cp.CustomerId);

        builder
            .HasOne(cp => cp.Promotion)
            .WithMany(p => p.CustomerPromotions)
            .HasForeignKey(cp => cp.PromotionId);

        builder
            .HasOne(cp => cp.Order)
            .WithMany()
            .HasForeignKey(cp => cp.UsedInOrderId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired(false);
    }
}