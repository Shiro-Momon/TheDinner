using FluentAssertions;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.UnitTests.Domain;

public class MenuItemTests
{
    [Fact]
    public void Should_CreateMenuItem_When_ValidData()
    {
        var item = MenuItem.Create("Burger", 12.50m, MenuItemCategory.MainCourse);

        item.Name.Should().Be("Burger");
        item.Price.Should().Be(12.50m);
        item.Category.Should().Be(MenuItemCategory.MainCourse);
        item.IsAvailable.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ThrowDomainException_When_NameIsEmpty(string name)
    {
        var act = () => MenuItem.Create(name, 10m, MenuItemCategory.Beverage);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_ThrowDomainException_When_PriceIsNotPositive(decimal price)
    {
        var act = () => MenuItem.Create("Water", price, MenuItemCategory.Beverage);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_UpdateMenuItem_When_ValidData()
    {
        var item = MenuItem.Create("Burger", 12.50m, MenuItemCategory.MainCourse);

        item.Update("Cheeseburger", 14m, MenuItemCategory.MainCourse, false);

        item.Name.Should().Be("Cheeseburger");
        item.Price.Should().Be(14m);
        item.IsAvailable.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_ThrowDomainException_When_UpdateNameIsEmpty(string name)
    {
        var item = MenuItem.Create("Burger", 12.50m, MenuItemCategory.MainCourse);

        var act = () => item.Update(name, 10m, MenuItemCategory.MainCourse, true);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Should_ThrowDomainException_When_UpdatePriceIsNotPositive(decimal price)
    {
        var item = MenuItem.Create("Burger", 12.50m, MenuItemCategory.MainCourse);

        var act = () => item.Update("Burger", price, MenuItemCategory.MainCourse, true);

        act.Should().Throw<DomainException>();
    }
}
