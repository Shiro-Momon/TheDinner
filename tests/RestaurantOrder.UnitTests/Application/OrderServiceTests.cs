using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RestaurantOrder.Application.DTOs.Orders;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Application.Services;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.UnitTests.Application;

public class OrderServiceTests
{
    private readonly IOrderRepository _orderRepo = Substitute.For<IOrderRepository>();
    private readonly IMenuItemRepository _menuRepo = Substitute.For<IMenuItemRepository>();
    private readonly IPricingStrategy _pricing = Substitute.For<IPricingStrategy>();
    private readonly OrderService _sut;

    public OrderServiceTests()
    {
        _sut = new OrderService(_orderRepo, _menuRepo, _pricing);
    }

    [Fact]
    public async Task Should_CreateOrder_When_ValidDto()
    {
        var tableId = Guid.NewGuid();
        var menuItemId = Guid.NewGuid();
        var menuItem = MenuItem.Create("Burger", 12m, MenuItemCategory.MainCourse);
        var dto = new CreateOrderDto(tableId, new List<CreateOrderItemDto>
        {
            new(menuItemId, 2),
        });

        _menuRepo.GetByIdAsync(menuItemId, Arg.Any<CancellationToken>())
            .Returns(menuItem);

        var result = await _sut.CreateAsync(dto);

        result.TableId.Should().Be(tableId);
        result.Items.Should().HaveCount(1);
        result.Status.Should().Be(OrderStatus.Pending);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_MenuItemNotFound()
    {
        var dto = new CreateOrderDto(Guid.NewGuid(), new List<CreateOrderItemDto>
        {
            new(Guid.NewGuid(), 1),
        });

        _menuRepo.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_MenuItemUnavailable()
    {
        var menuItem = MenuItem.Create("Sold Out", 5m, MenuItemCategory.Dessert, isAvailable: false);
        var dto = new CreateOrderDto(Guid.NewGuid(), new List<CreateOrderItemDto>
        {
            new(menuItem.Id, 1),
        });

        _menuRepo.GetByIdAsync(menuItem.Id, Arg.Any<CancellationToken>())
            .Returns(menuItem);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ConfirmOrder_When_OrderExists()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(OrderItem.Create(Guid.NewGuid(), 1, 10m));

        _orderRepo.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>())
            .Returns(order);

        var result = await _sut.ConfirmAsync(order.Id);

        result.Status.Should().Be(OrderStatus.Confirmed);
        await _orderRepo.Received(1).UpdateAsync(order, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_OrderNotFound()
    {
        _orderRepo.GetWithItemsAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _sut.ConfirmAsync(Guid.NewGuid());

        await act.Should().ThrowAsync<DomainException>();
    }
}
