using FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using RestaurantOrder.Application.EventHandlers;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Events;

namespace RestaurantOrder.UnitTests.Application;

public class EventHandlerTests
{
    [Fact]
    public async Task Should_CompleteSuccessfully_When_OrderConfirmedHandled()
    {
        var logger = Substitute.For<ILogger<OrderConfirmedEventHandler>>();
        var handler = new OrderConfirmedEventHandler(logger);
        var domainEvent = new OrderConfirmedEvent(1, 1, DateTimeOffset.UtcNow);

        var act = async () => await handler.HandleAsync(domainEvent);

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task Should_CompleteSuccessfully_When_OrderPaidHandled()
    {
        var logger = Substitute.For<ILogger<OrderPaidEventHandler>>();
        var handler = new OrderPaidEventHandler(logger);
        var domainEvent = new OrderPaidEvent(1, 50m, PaymentMethod.Cash, DateTimeOffset.UtcNow);

        var act = async () => await handler.HandleAsync(domainEvent);

        await act.Should().NotThrowAsync();
    }
}
