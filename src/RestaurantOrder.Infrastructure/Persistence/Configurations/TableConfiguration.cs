using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Infrastructure.Persistence.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Number).IsRequired();
        builder.Property(t => t.Capacity).IsRequired();
        builder.Property(t => t.IsOccupied).IsRequired();
        builder.HasIndex(t => t.Number).IsUnique();
    }
}
