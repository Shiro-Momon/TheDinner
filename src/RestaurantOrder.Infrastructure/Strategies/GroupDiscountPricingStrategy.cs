using System;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.Strategies;

/// <summary>
/// Applies a 10% discount when the order contains more than 8 items.
/// </summary>
public class GroupDiscountPricingStrategy : IPricingStrategy
{
    private const int GroupThreshold = 8;
    private const decimal DiscountRate = 0.10m;

    public string StrategyName => "GroupDiscount";

    public decimal CalculateTotal(IReadOnlyList<OrderItem> items, Func<int, MenuItemCategory> getCategory)
    {
        var subtotal = items.Sum(i => i.SubTotal);
        var totalQuantity = items.Sum(i => i.Quantity);

        return totalQuantity > GroupThreshold
            ? subtotal * (1 - DiscountRate)
            : subtotal;
    }
}
