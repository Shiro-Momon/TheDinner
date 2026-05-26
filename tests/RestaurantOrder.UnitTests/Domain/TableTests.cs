using FluentAssertions;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.UnitTests.Domain;

public class TableTests
{
    [Fact]
    public void Should_CreateTable_When_ValidData()
    {
        var table = Table.Create(1, 4);

        table.Number.Should().Be(1);
        table.Capacity.Should().Be(4);
        table.IsOccupied.Should().BeFalse();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ThrowDomainException_When_NumberIsNotPositive(int number)
    {
        var act = () => Table.Create(number, 4);

        act.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Should_ThrowDomainException_When_CapacityIsNotPositive(int capacity)
    {
        var act = () => Table.Create(1, capacity);

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_OccupyTable_When_TableIsFree()
    {
        var table = Table.Create(1, 4);

        table.Occupy();

        table.IsOccupied.Should().BeTrue();
    }

    [Fact]
    public void Should_ThrowDomainException_When_OccupyingAlreadyOccupiedTable()
    {
        var table = Table.Create(1, 4);
        table.Occupy();

        var act = () => table.Occupy();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_ReleaseTable_When_TableIsOccupied()
    {
        var table = Table.Create(1, 4);
        table.Occupy();

        table.Release();

        table.IsOccupied.Should().BeFalse();
    }
}
