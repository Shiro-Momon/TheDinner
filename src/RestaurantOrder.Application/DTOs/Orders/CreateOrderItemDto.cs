namespace RestaurantOrder.Application.DTOs.Orders;

public record CreateOrderItemDto(
    Guid MenuItemId,
    int Quantity,
    string? SpecialInstructions = null);
