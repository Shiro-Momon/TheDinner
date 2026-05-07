namespace RestaurantOrder.Application.DTOs.Orders;

public record AddOrderItemDto(
    int MenuItemId,
    int Quantity,
    string? SpecialInstructions = null);
