using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Domain.Events;

public sealed record OrderPaidEvent(
    int OrderId,
    decimal TotalAmount,
    PaymentMethod Method,
    DateTimeOffset OccurredAt) : IDomainEvent;
