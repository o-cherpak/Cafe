using CafeApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CafeApi.ModelConfigurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasOne(u => u.Customer)
            .WithOne()
            .HasForeignKey<User>(u => u.CustomerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasIndex(u => u.Email)
            .IsUnique();

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}