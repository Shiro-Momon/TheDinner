using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.IntegrationTests;

public class DataSeederTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly RestaurantDbContext _context;

    public DataSeederTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<RestaurantDbContext>()
            .UseSqlite(_connection)
            .Options;
        _context = new RestaurantDbContext(options);
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
        _connection.Dispose();
    }

    [Fact]
    public async Task Should_SeedMenuItemsAndTables_When_DatabaseIsEmpty()
    {
        await DataSeeder.SeedAsync(_context);

        _context.MenuItems.Should().NotBeEmpty();
        _context.Tables.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Should_NotReseed_When_DataAlreadyExists()
    {
        await DataSeeder.SeedAsync(_context);
        int countAfterFirst = await _context.MenuItems.CountAsync();

        await DataSeeder.SeedAsync(_context);

        int countAfterSecond = await _context.MenuItems.CountAsync();
        countAfterSecond.Should().Be(countAfterFirst);
    }
}
