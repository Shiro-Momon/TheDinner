namespace RestaurantOrder.Application.DTOs.Orders;

public record OrderItemResponseDto(
    Guid Id,
    Guid MenuItemId,
    int Quantity,
    decimal UnitPrice,
    decimal SubTotal,
    string? SpecialInstructions);
