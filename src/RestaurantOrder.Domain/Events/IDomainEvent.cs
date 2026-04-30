namespace RestaurantOrder.Domain.Events;

public interface IDomainEvent
{
    DateTimeOffset OccurredAt { get; }
}
