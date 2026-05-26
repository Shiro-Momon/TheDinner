using FluentAssertions;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Application.Services;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;
using RestaurantOrder.Infrastructure.PaymentProcessors;

namespace RestaurantOrder.UnitTests.Application;

public class PaymentFactoryTests
{
    [Fact]
    public void Should_ReturnCorrectProcessor_When_MethodIsRegistered()
    {
        var processors = new IPaymentProcessor[]
        {
            new CashPaymentProcessor(),
            new CreditCardPaymentProcessor(),
            new MealVoucherPaymentProcessor(),
        };
        var factory = new PaymentFactory(processors);

        factory.GetProcessor(PaymentMethod.Cash).SupportedMethod.Should().Be(PaymentMethod.Cash);
        factory.GetProcessor(PaymentMethod.CreditCard).SupportedMethod.Should().Be(PaymentMethod.CreditCard);
        factory.GetProcessor(PaymentMethod.MealVoucher).SupportedMethod.Should().Be(PaymentMethod.MealVoucher);
    }

    [Fact]
    public void Should_ThrowDomainException_When_NoProcessorRegistered()
    {
        var factory = new PaymentFactory(Array.Empty<IPaymentProcessor>());

        var act = () => factory.GetProcessor(PaymentMethod.Cash);

        act.Should().Throw<DomainException>();
    }
}
