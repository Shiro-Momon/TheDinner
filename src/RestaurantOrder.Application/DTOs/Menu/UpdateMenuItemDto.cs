using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Menu;

public record UpdateMenuItemDto(
    string Name,
    decimal Price,
    MenuItemCategory Category,
    bool IsAvailable);
