using RestaurantOrder.Domain.Events;

namespace RestaurantOrder.Application.Interfaces;

public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    Task HandleAsync(TEvent domainEvent, CancellationToken ct = default);
}
