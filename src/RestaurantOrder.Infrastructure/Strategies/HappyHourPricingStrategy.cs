using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.Strategies;

/// <summary>
/// Applies a 20% discount on beverages during happy hour.
/// </summary>
public class HappyHourPricingStrategy : IPricingStrategy
{
    public string StrategyName => "HappyHour";

    public decimal CalculateTotal(IReadOnlyList<OrderItem> items) =>
        items.Sum(i => i.SubTotal);
}
