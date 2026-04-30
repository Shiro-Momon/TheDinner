using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Application.Services;

public class PaymentFactory
{
    private readonly IEnumerable<IPaymentProcessor> _processors;

    public PaymentFactory(IEnumerable<IPaymentProcessor> processors)
    {
        _processors = processors;
    }

    public IPaymentProcessor GetProcessor(PaymentMethod method)
    {
        return _processors.FirstOrDefault(p => p.SupportedMethod == method)
            ?? throw new DomainException($"No payment processor registered for method '{method}'.");
    }
}
