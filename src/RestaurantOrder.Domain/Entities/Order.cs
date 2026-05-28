using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Events;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Domain.Entities;

public class Order
{
    private readonly List<OrderItem> _items = new();
    private readonly List<IDomainEvent> _domainEvents = new();

    private Order()
    {
    }

    public int Id { get; private set; }
    public int? TableId { get; private set; }
    public bool IsToGo { get; private set; }
    public PricingStrategyType PricingStrategy { get; private set; }
    public string? CustomerName { get; private set; }
    public OrderStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public DateTimeOffset? ServedAt { get; private set; }
    public DateTimeOffset? PaidAt { get; private set; }

    public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public decimal? FinalTotal { get; private set; }
    public decimal TotalAmount => FinalTotal ?? _items.Sum(i => i.SubTotal);

    public void SetFinalTotal(decimal total)
    {
        FinalTotal = total;
    }

    public static Order Create(int? tableId, bool isToGo, PricingStrategyType pricingStrategy, string? customerName = null)
    {
        if (!isToGo && tableId is null)
            throw new DomainException("A table must be specified for dine-in orders.");

        return new Order
        {
            TableId = tableId,
            IsToGo = isToGo,
            PricingStrategy = pricingStrategy,
            CustomerName = string.IsNullOrWhiteSpace(customerName) ? null : customerName.Trim(),
            Status = OrderStatus.Pending,
            CreatedAt = DateTimeOffset.UtcNow,
        };
    }

    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Items can only be added to a pending order.");

        _items.Add(item);
    }

    public void RemoveItem(int menuItemId)
    {
        if (Status != OrderStatus.Pending)
            throw new DomainException("Items can only be removed from a pending order.");

        var item = _items.FirstOrDefault(i => i.MenuItemId == menuItemId)
            ?? throw new DomainException($"Item with MenuItemId '{menuItemId}' not found in order.");

        _items.Remove(item);
    }

    public void Confirm()
    {
        EnsureStatus(OrderStatus.Pending);
        if (_items.Count == 0)
            throw new DomainException("Cannot confirm an order with no items.");

        Status = OrderStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
        _domainEvents.Add(new OrderConfirmedEvent(Id, TableId, DateTimeOffset.UtcNow));
    }

    public void StartPreparing()
    {
        EnsureStatus(OrderStatus.Confirmed);
        Status = OrderStatus.Preparing;
    }

    public void MarkReady()
    {
        EnsureStatus(OrderStatus.Preparing);
        Status = OrderStatus.Ready;
    }

    public void Serve()
    {
        EnsureStatus(OrderStatus.Ready);
        Status = OrderStatus.Served;
        ServedAt = DateTimeOffset.UtcNow;
    }

    public void MarkPaid(PaymentMethod method)
    {
        if (Status == OrderStatus.Paid)
            throw new DomainException("Order is already paid.");
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Cannot pay a cancelled order.");

        Status = OrderStatus.Paid;
        PaidAt = DateTimeOffset.UtcNow;
        _domainEvents.Add(new OrderPaidEvent(Id, TotalAmount, method, DateTimeOffset.UtcNow));
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Paid)
            throw new DomainException("Cannot cancel a paid order.");
        if (Status == OrderStatus.Cancelled)
            throw new DomainException("Order is already cancelled.");

        Status = OrderStatus.Cancelled;
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    private void EnsureStatus(OrderStatus expected)
    {
        if (Status != expected)
            throw new InvalidOrderStatusTransitionException(Status, expected);
    }
}
