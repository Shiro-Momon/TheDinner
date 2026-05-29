using System;
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

    public decimal CalculateTotal(IReadOnlyList<OrderItem> items, Func<int, MenuItemCategory> getCategory)
    {
        return items.Sum(i =>
        {
            var isBeverage = getCategory(i.MenuItemId) == MenuItemCategory.Beverage;
            var price = isBeverage ? i.UnitPrice * 0.8m : i.UnitPrice;
            return i.Quantity * price;
        });
    }
}
