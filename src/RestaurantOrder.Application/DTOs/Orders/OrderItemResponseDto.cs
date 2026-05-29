namespace RestaurantOrder.Application.DTOs.Orders;

public record OrderItemResponseDto(
    int Id,
    int MenuItemId,
    string MenuItemName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal,
    string? SpecialInstructions);
