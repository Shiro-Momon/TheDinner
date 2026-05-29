using System;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.Strategies;

public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard";

    public decimal CalculateTotal(IReadOnlyList<OrderItem> items, Func<int, MenuItemCategory> getCategory) =>
        items.Sum(i => i.SubTotal);
}
