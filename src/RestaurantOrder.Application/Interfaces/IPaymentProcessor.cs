using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.Interfaces;

public interface IPaymentProcessor
{
    PaymentMethod SupportedMethod { get; }
    Task<PaymentResult> ProcessAsync(decimal amount, CancellationToken ct = default);
}
