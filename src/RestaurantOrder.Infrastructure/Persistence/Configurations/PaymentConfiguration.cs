using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.OrderId).IsRequired();
        builder.Property(p => p.Amount).IsRequired().HasPrecision(10, 2);
        builder.Property(p => p.TipAmount).IsRequired().HasPrecision(10, 2);
        builder.Property(p => p.Method).IsRequired();
        builder.Property(p => p.TransactionReference).IsRequired().HasMaxLength(200);
        builder.Property(p => p.ProcessedAt).IsRequired();
        builder.HasIndex(p => p.OrderId).IsUnique();
    }
}
