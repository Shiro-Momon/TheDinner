using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(RestaurantDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<Order>> GetByStatusAsync(OrderStatus status, CancellationToken ct = default) =>
        await DbSet.Include(o => o.Items).Where(o => o.Status == status).ToListAsync(ct);

    public async Task<Order?> GetWithItemsAsync(Guid orderId, CancellationToken ct = default) =>
        await DbSet.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == orderId, ct);
}
