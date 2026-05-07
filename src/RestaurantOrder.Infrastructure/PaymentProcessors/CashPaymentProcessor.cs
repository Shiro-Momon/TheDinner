using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.PaymentProcessors;

public class CashPaymentProcessor : IPaymentProcessor
{
    public PaymentMethod SupportedMethod => PaymentMethod.Cash;

    public Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct = default)
    {
        // Cash payments are always accepted.
        // Decline case: amount <= 0 (guard against invalid calls).
        if (amount <= 0)
            return Task.FromResult(PaymentResult.Declined("Payment amount must be greater than zero."));

        return Task.FromResult(PaymentResult.Success($"CASH-{Guid.NewGuid():N}"));
    }
}
