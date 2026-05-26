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
        var menuItem = MenuItem.Create("Burger", 12m, MenuItemCategory.MainCourse);
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto>
        {
            new(1, 2),
        });

        _menuRepo.GetByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(menuItem);

        var result = await _sut.CreateAsync(dto);

        result.TableId.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Status.Should().Be(OrderStatus.Pending);
        await _orderRepo.Received(1).AddAsync(Arg.Any<Order>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_MenuItemNotFound()
    {
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto>
        {
            new(99, 1),
        });

        _menuRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_MenuItemUnavailable()
    {
        var menuItem = MenuItem.Create("Sold Out", 5m, MenuItemCategory.Dessert, isAvailable: false);
        var dto = new CreateOrderDto(1, new List<CreateOrderItemDto>
        {
            new(1, 1),
        });

        _menuRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(menuItem);

        var act = async () => await _sut.CreateAsync(dto);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ConfirmOrder_When_OrderExists()
    {
        var order = Order.Create(1);
        order.AddItem(OrderItem.Create(1, 1, 10m));

        _orderRepo.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>())
            .Returns(order);

        var result = await _sut.ConfirmAsync(order.Id);

        result.Status.Should().Be(OrderStatus.Confirmed);
        await _orderRepo.Received(1).UpdateAsync(order, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_OrderNotFound()
    {
        _orderRepo.GetWithItemsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .ReturnsNull();

        var act = async () => await _sut.ConfirmAsync(99);

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ReturnAllOrders_When_NoStatusFilter()
    {
        _orderRepo.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(new List<Order>());

        var result = await _sut.GetAllAsync();

        result.Should().BeEmpty();
        await _orderRepo.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ReturnFilteredOrders_When_StatusProvided()
    {
        _orderRepo.GetByStatusAsync(OrderStatus.Pending, Arg.Any<CancellationToken>())
            .Returns(new List<Order>());

        var result = await _sut.GetAllAsync(OrderStatus.Pending);

        result.Should().BeEmpty();
        await _orderRepo.Received(1).GetByStatusAsync(OrderStatus.Pending, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_AddItem_When_OrderAndMenuItemExist()
    {
        var order = Order.Create(1);
        order.AddItem(OrderItem.Create(1, 1, 5m));
        var menuItem = MenuItem.Create("Fries", 5m, MenuItemCategory.Side);
        var dto = new AddOrderItemDto(2, 1);

        _orderRepo.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _menuRepo.GetByIdAsync(2, Arg.Any<CancellationToken>()).Returns(menuItem);

        var result = await _sut.AddItemAsync(order.Id, dto);

        result.Items.Should().HaveCount(2);
        await _orderRepo.Received(1).UpdateAsync(order, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_AddItemOrderNotFound()
    {
        _orderRepo.GetWithItemsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _sut.AddItemAsync(99, new AddOrderItemDto(1, 1));

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_AddItemMenuItemNotFound()
    {
        var order = Order.Create(1);
        _orderRepo.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _menuRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _sut.AddItemAsync(order.Id, new AddOrderItemDto(99, 1));

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_AddItemMenuItemUnavailable()
    {
        var order = Order.Create(1);
        var menuItem = MenuItem.Create("Unavailable", 5m, MenuItemCategory.Side, isAvailable: false);
        _orderRepo.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);
        _menuRepo.GetByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(menuItem);

        var act = async () => await _sut.AddItemAsync(order.Id, new AddOrderItemDto(1, 1));

        await act.Should().ThrowAsync<DomainException>();
    }

    [Fact]
    public async Task Should_RemoveItem_When_OrderAndItemExist()
    {
        var order = Order.Create(1);
        order.AddItem(OrderItem.Create(1, 1, 10m));

        _orderRepo.GetWithItemsAsync(order.Id, Arg.Any<CancellationToken>()).Returns(order);

        var result = await _sut.RemoveItemAsync(order.Id, 1);

        result.Items.Should().BeEmpty();
        await _orderRepo.Received(1).UpdateAsync(order, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Should_ThrowDomainException_When_RemoveItemOrderNotFound()
    {
        _orderRepo.GetWithItemsAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).ReturnsNull();

        var act = async () => await _sut.RemoveItemAsync(99, 1);

        await act.Should().ThrowAsync<DomainException>();
    }
}
