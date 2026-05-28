using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.TableId).IsRequired(false);
        builder.Property(o => o.IsToGo).IsRequired();
        builder.Property(o => o.PricingStrategy).IsRequired();
        builder.Property(o => o.FinalTotal).IsRequired(false);
        builder.Property(o => o.CustomerName).IsRequired(false).HasMaxLength(100);
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired();

        builder.Ignore(o => o.TotalAmount);
        builder.Ignore(o => o.DomainEvents);

        builder.HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(o => o.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasField("_items");
    }
}
