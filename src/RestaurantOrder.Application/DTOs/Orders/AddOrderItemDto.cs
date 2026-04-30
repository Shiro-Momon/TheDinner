namespace RestaurantOrder.Application.DTOs.Orders;

public record AddOrderItemDto(
    Guid MenuItemId,
    int Quantity,
    string? SpecialInstructions = null);
