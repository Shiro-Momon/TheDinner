using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Menu;

public record CreateMenuItemDto(
    string Name,
    decimal Price,
    MenuItemCategory Category,
    bool IsAvailable = true,
    string? ImageUrl = null);
