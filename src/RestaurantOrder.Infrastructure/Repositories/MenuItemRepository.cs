using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.Infrastructure.Repositories;

public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
{
    public MenuItemRepository(RestaurantDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<MenuItem>> GetByCategoryAsync(MenuItemCategory category, CancellationToken ct = default) =>
        await DbSet.Where(m => m.Category == category).ToListAsync(ct);

    public async Task<IReadOnlyList<MenuItem>> GetAvailableAsync(CancellationToken ct = default) =>
        await DbSet.Where(m => m.IsAvailable).ToListAsync(ct);
}
