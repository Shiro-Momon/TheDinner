using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Infrastructure.Persistence;

public class RestaurantDbContext : DbContext
{
    public RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : base(options)
    {
    }

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Payment> Payments => Set<Payment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(RestaurantDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
