using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Menu;

public record MenuItemResponseDto(
    int Id,
    string Name,
    decimal Price,
    MenuItemCategory Category,
    bool IsAvailable,
    string? ImageUrl);
