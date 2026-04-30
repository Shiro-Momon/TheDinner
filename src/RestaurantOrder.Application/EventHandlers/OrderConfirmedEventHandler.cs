using Microsoft.Extensions.Logging;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Events;

namespace RestaurantOrder.Application.EventHandlers;

public class OrderConfirmedEventHandler : IDomainEventHandler<OrderConfirmedEvent>
{
    private readonly ILogger<OrderConfirmedEventHandler> _logger;

    public OrderConfirmedEventHandler(ILogger<OrderConfirmedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(OrderConfirmedEvent domainEvent, CancellationToken ct = default)
    {
        _logger.LogInformation(
            "Order {OrderId} confirmed for table {TableId} at {OccurredAt}. Kitchen notified.",
            domainEvent.OrderId,
            domainEvent.TableId,
            domainEvent.OccurredAt);

        return Task.CompletedTask;
    }
}
