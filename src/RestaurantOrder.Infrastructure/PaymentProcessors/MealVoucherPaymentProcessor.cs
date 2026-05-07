using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.PaymentProcessors;

public class MealVoucherPaymentProcessor : IPaymentProcessor
{
    private const decimal MaxCoverageAmount = 25.00m;

    public PaymentMethod SupportedMethod => PaymentMethod.MealVoucher;

    public Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct = default)
    {
        // Decline cases:
        // - amount <= 0: invalid amount
        // - amount > MaxCoverageAmount: meal vouchers are capped per French regulation (Titre-Restaurant)
        // - tip included: meal vouchers cannot cover tips
        if (amount <= 0)
            return Task.FromResult(PaymentResult.Declined("Payment amount must be greater than zero."));

        if (amount > MaxCoverageAmount)
            return Task.FromResult(
                PaymentResult.Declined($"Meal voucher maximum coverage is {MaxCoverageAmount:C}. Amount {amount:C} exceeds the limit."));

        // Simulated as always approved — replace with Edenred/Sodexo API call in production.
        return Task.FromResult(PaymentResult.Success($"MV-{Guid.NewGuid():N}"));
    }
}
