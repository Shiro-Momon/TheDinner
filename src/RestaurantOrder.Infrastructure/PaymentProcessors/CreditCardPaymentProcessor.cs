using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Infrastructure.PaymentProcessors;

public class CreditCardPaymentProcessor : IPaymentProcessor
{
    public PaymentMethod SupportedMethod => PaymentMethod.CreditCard;

    public Task<string> ProcessAsync(decimal amount, CancellationToken ct = default) =>
        Task.FromResult($"CC-{Guid.NewGuid():N}");
}
