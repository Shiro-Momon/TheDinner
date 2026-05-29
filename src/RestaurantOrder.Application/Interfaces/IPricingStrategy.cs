using System;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.Interfaces;

public interface IPricingStrategy
{
    string StrategyName { get; }
    decimal CalculateTotal(IReadOnlyList<OrderItem> items, Func<int, MenuItemCategory> getCategory);
}
