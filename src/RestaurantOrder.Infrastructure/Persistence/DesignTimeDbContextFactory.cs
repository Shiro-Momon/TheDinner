using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RestaurantOrder.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<RestaurantDbContext>
{
    public RestaurantDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseNpgsql("Host=localhost;Database=restaurantorder;Username=postgres;Password=postgres")
            .Options;

        return new RestaurantDbContext(options);
    }
}
