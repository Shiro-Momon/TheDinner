using Microsoft.Extensions.Logging;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Events;

namespace RestaurantOrder.Application.EventHandlers;

public class OrderPaidEventHandler : IDomainEventHandler<OrderPaidEvent>
{
    private readonly ILogger<OrderPaidEventHandler> _logger;

    public OrderPaidEventHandler(ILogger<OrderPaidEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderPaidEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Order {OrderId} paid — amount {TotalAmount:C} via {Method} at {OccurredAt}.",
            domainEvent.OrderId,
            domainEvent.TotalAmount,
            domainEvent.Method,
            domainEvent.OccurredAt);

        return Task.CompletedTask;
    }
}
