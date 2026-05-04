using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Infrastructure.Strategies;

public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard";

    public decimal CalculateTotal(IReadOnlyList<OrderItem> items) =>
        items.Sum(i => i.SubTotal);
}
