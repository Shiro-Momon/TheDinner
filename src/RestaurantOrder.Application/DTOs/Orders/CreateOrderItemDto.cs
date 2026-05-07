namespace RestaurantOrder.Application.DTOs.Orders;

public record CreateOrderItemDto(
    int MenuItemId,
    int Quantity,
    string? SpecialInstructions = null);
