using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove DbContextOptions and any IDbContextOptionsConfiguration for RestaurantDbContext
            var toRemove = services
                .Where(d =>
                    d.ServiceType == typeof(DbContextOptions<RestaurantDbContext>) ||
                    (d.ServiceType.IsGenericType &&
                     d.ServiceType.GetGenericTypeDefinition() == typeof(IDbContextOptionsConfiguration<>) &&
                     d.ServiceType.GenericTypeArguments[0] == typeof(RestaurantDbContext)))
                .ToList();

            foreach (var d in toRemove)
                services.Remove(d);

            _connection.Open();

            services.AddDbContext<RestaurantDbContext>(options =>
                options.UseSqlite(_connection).EnableServiceProviderCaching(false));
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
            _connection.Dispose();
    }
}
