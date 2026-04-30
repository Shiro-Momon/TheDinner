using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Application.Interfaces;

public interface IPricingStrategy
{
    string StrategyName { get; }
    decimal CalculateTotal(IReadOnlyList<OrderItem> items);
}
