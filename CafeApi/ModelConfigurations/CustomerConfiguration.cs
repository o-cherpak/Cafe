using CafeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeApi.ModelConfigurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder
            .HasIndex(c => c.Email)
            .IsUnique();

        builder
            .Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(140);

        builder
            .Property(c => c.Email)
            .IsRequired()
            .HasMaxLength(200);
    }
}