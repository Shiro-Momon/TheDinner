using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Domain.Events;

public sealed record OrderPaidEvent(
    Guid OrderId,
    decimal TotalAmount,
    PaymentMethod Method,
    DateTimeOffset OccurredAt) : IDomainEvent;
