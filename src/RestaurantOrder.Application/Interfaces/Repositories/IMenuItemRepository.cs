using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.Interfaces.Repositories;

public interface IMenuItemRepository : IRepository<MenuItem>
{
    Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(MenuItemCategory category, CancellationToken ct = default);
    Task<IReadOnlyList<MenuItem>> GetAvailableAsync(CancellationToken ct = default);
}
