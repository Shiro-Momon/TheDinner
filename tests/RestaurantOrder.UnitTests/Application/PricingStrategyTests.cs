using FluentAssertions;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Infrastructure.Strategies;

namespace RestaurantOrder.UnitTests.Application;

public class PricingStrategyTests
{
    private static IReadOnlyList<OrderItem> MakeItems(int count, decimal unitPrice) =>
        Enumerable.Range(1, count)
            .Select(i => OrderItem.Create(i, 1, unitPrice))
            .ToList();

    [Fact]
    public void Should_ReturnSumOfSubTotals_When_StandardStrategy()
    {
        var strategy = new StandardPricingStrategy();
        var items = MakeItems(3, 10m);

        var total = strategy.CalculateTotal(items);

        total.Should().Be(30m);
    }

    [Fact]
    public void Should_ApplyTenPercentDiscount_When_GroupDiscountAndMoreThanEightItems()
    {
        var strategy = new GroupDiscountPricingStrategy();
        var items = MakeItems(9, 10m);

        var total = strategy.CalculateTotal(items);

        total.Should().Be(81m);
    }

    [Fact]
    public void Should_NotApplyDiscount_When_GroupDiscountAndEightOrFewerItems()
    {
        var strategy = new GroupDiscountPricingStrategy();
        var items = MakeItems(8, 10m);

        var total = strategy.CalculateTotal(items);

        total.Should().Be(80m);
    }

    [Fact]
    public void Should_HaveCorrectStrategyName_When_Standard()
    {
        new StandardPricingStrategy().StrategyName.Should().Be("Standard");
    }

    [Fact]
    public void Should_HaveCorrectStrategyName_When_HappyHour()
    {
        new HappyHourPricingStrategy().StrategyName.Should().Be("HappyHour");
    }

    [Fact]
    public void Should_HaveCorrectStrategyName_When_GroupDiscount()
    {
        new GroupDiscountPricingStrategy().StrategyName.Should().Be("GroupDiscount");
    }

    [Fact]
    public void Should_ReturnSumOfSubTotals_When_HappyHourStrategy()
    {
        var strategy = new HappyHourPricingStrategy();
        var items = MakeItems(3, 10m);

        var total = strategy.CalculateTotal(items);

        total.Should().Be(30m);
    }
}
