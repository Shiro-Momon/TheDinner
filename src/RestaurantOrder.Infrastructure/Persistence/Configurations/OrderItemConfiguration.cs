using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.MenuItemId).IsRequired();
        builder.Property(i => i.Quantity).IsRequired();
        builder.Property(i => i.UnitPrice).IsRequired().HasPrecision(10, 2);
        builder.Property(i => i.SpecialInstructions).HasMaxLength(500);
        builder.Ignore(i => i.SubTotal);
    }
}
