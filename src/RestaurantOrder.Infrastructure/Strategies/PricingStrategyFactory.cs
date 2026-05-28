using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.Strategies;

public class PricingStrategyFactory : IPricingStrategyFactory
{
    private readonly StandardPricingStrategy _standard;
    private readonly HappyHourPricingStrategy _happyHour;
    private readonly GroupDiscountPricingStrategy _groupDiscount;

    public PricingStrategyFactory(
        StandardPricingStrategy standard,
        HappyHourPricingStrategy happyHour,
        GroupDiscountPricingStrategy groupDiscount)
    {
        _standard = standard;
        _happyHour = happyHour;
        _groupDiscount = groupDiscount;
    }

    public IPricingStrategy GetStrategy(PricingStrategyType type) => type switch
    {
        PricingStrategyType.HappyHour => _happyHour,
        PricingStrategyType.GroupDiscount => _groupDiscount,
        _ => _standard,
    };
}
