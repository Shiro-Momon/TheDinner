using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.PaymentProcessors;

public class CashPaymentProcessor : IPaymentProcessor
{
    public PaymentMethod SupportedMethod => PaymentMethod.Cash;

    public Task<string> ProcessAsync(decimal amount, CancellationToken ct = default) =>
        Task.FromResult($"CASH-{Guid.NewGuid():N}");
}
