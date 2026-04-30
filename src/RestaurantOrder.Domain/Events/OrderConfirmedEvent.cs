namespace RestaurantOrder.Domain.Events;

public sealed record OrderConfirmedEvent(
    Guid OrderId,
    Guid TableId,
    DateTimeOffset OccurredAt) : IDomainEvent;
