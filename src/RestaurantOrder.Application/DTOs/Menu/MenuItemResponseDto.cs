using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Menu;

public record MenuItemResponseDto(
    Guid Id,
    string Name,
    decimal Price,
    MenuItemCategory Category,
    bool IsAvailable);
