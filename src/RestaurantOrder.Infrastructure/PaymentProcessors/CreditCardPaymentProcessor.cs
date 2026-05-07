using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.PaymentProcessors;

public class CreditCardPaymentProcessor : IPaymentProcessor
{
    public PaymentMethod SupportedMethod => PaymentMethod.CreditCard;

    public Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct = default)
    {
        // Decline cases (would apply when connected to a real payment gateway):
        // - amount <= 0: invalid amount
        // - amount > 2000: transaction limit exceeded
        // - gateway returns a decline code (insufficient funds, expired card, etc.)
        if (amount <= 0)
            return Task.FromResult(PaymentResult.Declined("Payment amount must be greater than zero."));

        if (amount > 2000)
            return Task.FromResult(PaymentResult.Declined("Transaction limit of 2000 exceeded."));

        // Simulated as always approved — replace with real gateway call in production.
        return Task.FromResult(PaymentResult.Success($"CC-{Guid.NewGuid():N}"));
    }
}
