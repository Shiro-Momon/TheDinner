namespace RestaurantOrder.Application.DTOs.Orders;

public record OrderItemResponseDto(
    int Id,
    int MenuItemId,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal,
    string? SpecialInstructions);
