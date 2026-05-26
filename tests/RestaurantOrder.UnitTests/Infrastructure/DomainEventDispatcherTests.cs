using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RestaurantOrder.Application.EventHandlers;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Events;
using RestaurantOrder.Infrastructure.EventDispatching;

namespace RestaurantOrder.UnitTests.Infrastructure;

public class DomainEventDispatcherTests
{
    private static IServiceProvider BuildProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddScoped<IDomainEventHandler<OrderConfirmedEvent>, OrderConfirmedEventHandler>();
        services.AddScoped<IDomainEventHandler<OrderPaidEvent>, OrderPaidEventHandler>();
        return services.BuildServiceProvider();
    }

    [Fact]
    public async Task Should_DispatchEvents_And_ClearThem_When_OrderHasEvents()
    {
        var dispatcher = new DomainEventDispatcher(BuildProvider());
        var order = Order.Create(1);
        order.AddItem(OrderItem.Create(1, 1, 10m));
        order.Confirm();
        order.DomainEvents.Should().NotBeEmpty();

        await dispatcher.DispatchAsync(order);

        order.DomainEvents.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_CompleteWithoutError_When_OrderHasNoEvents()
    {
        var dispatcher = new DomainEventDispatcher(BuildProvider());
        var order = Order.Create(1);

        var act = async () => await dispatcher.DispatchAsync(order);

        await act.Should().NotThrowAsync();
    }
}
