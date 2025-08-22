using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Primary key
        builder.HasKey(o => o.Id);

        // Indexes
        builder.HasIndex(o => o.CreatedDate);
        builder.HasIndex(o => o.Status);

        // Column configs
        builder.Property(o => o.Id)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Name)
            .HasMaxLength(200);

        builder.Property(o => o.Amount)
            .HasColumnType("decimal(18,2)");
        
        builder.Property(o => o.Note)
            .IsRequired(false);

        builder.Property(o => o.CallbackUrl)
            .IsRequired(false);

        builder.Property(o => o.IpnUrl)
            .IsRequired(false);

        builder.Property(o => o.ErrorMessage)
            .IsRequired(false);
    }
}