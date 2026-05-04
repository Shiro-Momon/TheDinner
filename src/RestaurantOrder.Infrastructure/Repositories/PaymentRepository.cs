using Microsoft.EntityFrameworkCore;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Infrastructure.Persistence;

namespace RestaurantOrder.Infrastructure.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(RestaurantDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default) =>
        await DbSet.FirstOrDefaultAsync(p => p.OrderId == orderId, ct);
}
