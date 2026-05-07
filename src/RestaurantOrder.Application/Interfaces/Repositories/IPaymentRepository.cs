using RestaurantOrder.Domain.Entities;

namespace RestaurantOrder.Application.Interfaces.Repositories;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(int orderId, CancellationToken ct = default);
}
