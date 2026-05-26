using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Infrastructure.Persistence;
using RestaurantOrder.Infrastructure.Repositories;

namespace RestaurantOrder.IntegrationTests;

public class RepositoryTests : IDisposable
{
    private readonly SqliteConnection _connection;
    private readonly RestaurantDbContext _context;

    public RepositoryTests()
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
    public async Task Should_ReturnOnlyAvailableItems_When_GetAvailableAsync()
    {
        var repo = new MenuItemRepository(_context);
        await repo.AddAsync(MenuItem.Create("Burger", 10m, MenuItemCategory.MainCourse, true));
        await repo.AddAsync(MenuItem.Create("Sold Out", 5m, MenuItemCategory.Side, false));

        var result = await repo.GetAvailableAsync();

        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Burger");
    }

    [Fact]
    public async Task Should_ReturnOrdersByStatus_When_GetByStatusAsync()
    {
        var repo = new OrderRepository(_context);
        var order = Order.Create(1);
        order.AddItem(OrderItem.Create(1, 1, 10m));
        await repo.AddAsync(order);

        var result = await repo.GetByStatusAsync(OrderStatus.Pending);

        result.Should().HaveCount(1);
        result[0].Status.Should().Be(OrderStatus.Pending);
    }
}
