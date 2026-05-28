using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.Interfaces;

public interface IPricingStrategyFactory
{
    IPricingStrategy GetStrategy(PricingStrategyType type);
}
