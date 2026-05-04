using FluentAssertions;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Events;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.UnitTests.Domain;

public class OrderTests
{
    private static OrderItem MakeItem(decimal price = 10m) =>
        OrderItem.Create(Guid.NewGuid(), 1, price);

    [Fact]
    public void Should_CreateOrder_When_ValidTableId()
    {
        var tableId = Guid.NewGuid();

        var order = Order.Create(tableId);

        order.TableId.Should().Be(tableId);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Should_AddItem_When_OrderIsPending()
    {
        var order = Order.Create(Guid.NewGuid());
        var item = MakeItem();

        order.AddItem(item);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(10m);
    }

    [Fact]
    public void Should_ThrowDomainException_When_AddingItemToConfirmedOrder()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());
        order.Confirm();

        var act = () => order.AddItem(MakeItem());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_RemoveItem_When_OrderIsPending()
    {
        var menuItemId = Guid.NewGuid();
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(OrderItem.Create(menuItemId, 1, 10m));

        order.RemoveItem(menuItemId);

        order.Items.Should().BeEmpty();
    }

    [Fact]
    public void Should_ThrowDomainException_When_RemovingNonExistentItem()
    {
        var order = Order.Create(Guid.NewGuid());

        var act = () => order.RemoveItem(Guid.NewGuid());

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_ConfirmOrder_When_HasItems()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
        order.ConfirmedAt.Should().NotBeNull();
    }

    [Fact]
    public void Should_ThrowDomainException_When_ConfirmingEmptyOrder()
    {
        var order = Order.Create(Guid.NewGuid());

        var act = () => order.Confirm();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_RaiseOrderConfirmedEvent_When_Confirmed()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());

        order.Confirm();

        order.DomainEvents.Should().ContainSingle(e => e is OrderConfirmedEvent);
    }

    [Fact]
    public void Should_FollowFullLifecycle_When_ValidTransitions()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());

        order.Confirm();
        order.StartPreparing();
        order.MarkReady();
        order.Serve();

        order.Status.Should().Be(OrderStatus.Served);
        order.ServedAt.Should().NotBeNull();
    }

    [Fact]
    public void Should_ThrowInvalidOrderStatusTransitionException_When_WrongTransition()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());

        var act = () => order.StartPreparing();

        act.Should().Throw<InvalidOrderStatusTransitionException>();
    }

    [Fact]
    public void Should_MarkPaid_And_RaiseEvent_When_Served()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem(20m));
        order.Confirm();
        order.StartPreparing();
        order.MarkReady();
        order.Serve();

        order.MarkPaid(PaymentMethod.Cash);

        order.Status.Should().Be(OrderStatus.Paid);
        order.PaidAt.Should().NotBeNull();
        order.DomainEvents.Should().Contain(e => e is OrderPaidEvent);
    }

    [Fact]
    public void Should_Cancel_When_OrderIsNotPaid()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());
        order.Confirm();

        order.Cancel();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Should_ThrowDomainException_When_CancellingPaidOrder()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());
        order.Confirm();
        order.StartPreparing();
        order.MarkReady();
        order.Serve();
        order.MarkPaid(PaymentMethod.Cash);

        var act = () => order.Cancel();

        act.Should().Throw<DomainException>();
    }

    [Fact]
    public void Should_ClearDomainEvents_When_Called()
    {
        var order = Order.Create(Guid.NewGuid());
        order.AddItem(MakeItem());
        order.Confirm();

        order.ClearDomainEvents();

        order.DomainEvents.Should().BeEmpty();
    }
}
