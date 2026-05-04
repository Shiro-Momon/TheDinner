using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.PaymentProcessors;

public class MealVoucherPaymentProcessor : IPaymentProcessor
{
    public PaymentMethod SupportedMethod => PaymentMethod.MealVoucher;

    public Task<string> ProcessAsync(decimal amount, CancellationToken ct = default) =>
        Task.FromResult($"MV-{Guid.NewGuid():N}");
}
