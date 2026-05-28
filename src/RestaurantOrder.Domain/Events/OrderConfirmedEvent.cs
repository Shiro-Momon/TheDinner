namespace RestaurantOrder.Domain.Events;

public sealed record OrderConfirmedEvent(
    int OrderId,
    int? TableId,
    DateTimeOffset OccurredAt) : IDomainEvent;
