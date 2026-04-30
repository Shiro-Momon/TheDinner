namespace RestaurantOrder.Domain.Entities;

public class OrderItem
{
    private OrderItem()
    {
    }

    public Guid Id { get; private set; }
    public Guid MenuItemId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }
    public string? SpecialInstructions { get; private set; }

    public decimal SubTotal => Quantity * UnitPrice;

    public static OrderItem Create(Guid menuItemId, int quantity, decimal unitPrice, string? specialInstructions = null)
    {
        return new OrderItem
        {
            Id = Guid.NewGuid(),
            MenuItemId = menuItemId,
            Quantity = quantity,
            UnitPrice = unitPrice,
            SpecialInstructions = specialInstructions,
        };
    }
}
