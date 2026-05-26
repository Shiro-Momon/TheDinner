using FluentAssertions;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Infrastructure.PaymentProcessors;

namespace RestaurantOrder.UnitTests.Infrastructure;

public class PaymentProcessorTests
{
    [Fact]
    public async Task Should_Succeed_When_CashPaymentWithPositiveAmount()
    {
        var result = await new CashPaymentProcessor().ProcessAsync(50m);

        result.IsSuccess.Should().BeTrue();
        result.TransactionReference.Should().StartWith("CASH-");
    }

    [Fact]
    public async Task Should_Decline_When_CashPaymentAmountIsZero()
    {
        var result = await new CashPaymentProcessor().ProcessAsync(0m);

        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Should_SupportCash_When_CashProcessor()
    {
        new CashPaymentProcessor().SupportedMethod.Should().Be(PaymentMethod.Cash);
    }

    [Fact]
    public async Task Should_Succeed_When_CreditCardPaymentWithValidAmount()
    {
        var result = await new CreditCardPaymentProcessor().ProcessAsync(100m);

        result.IsSuccess.Should().BeTrue();
        result.TransactionReference.Should().StartWith("CC-");
    }

    [Fact]
    public async Task Should_Decline_When_CreditCardPaymentAmountIsZero()
    {
        var result = await new CreditCardPaymentProcessor().ProcessAsync(0m);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Decline_When_CreditCardPaymentExceedsLimit()
    {
        var result = await new CreditCardPaymentProcessor().ProcessAsync(2001m);

        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().Contain("2000");
    }

    [Fact]
    public void Should_SupportCreditCard_When_CreditCardProcessor()
    {
        new CreditCardPaymentProcessor().SupportedMethod.Should().Be(PaymentMethod.CreditCard);
    }

    [Fact]
    public async Task Should_Succeed_When_MealVoucherWithValidAmount()
    {
        var result = await new MealVoucherPaymentProcessor().ProcessAsync(20m);

        result.IsSuccess.Should().BeTrue();
        result.TransactionReference.Should().StartWith("MV-");
    }

    [Fact]
    public async Task Should_Decline_When_MealVoucherAmountIsZero()
    {
        var result = await new MealVoucherPaymentProcessor().ProcessAsync(0m);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task Should_Decline_When_MealVoucherExceedsMaxCoverage()
    {
        var result = await new MealVoucherPaymentProcessor().ProcessAsync(30m);

        result.IsSuccess.Should().BeFalse();
        result.FailureReason.Should().Contain("25");
    }

    [Fact]
    public void Should_SupportMealVoucher_When_MealVoucherProcessor()
    {
        new MealVoucherPaymentProcessor().SupportedMethod.Should().Be(PaymentMethod.MealVoucher);
    }
}
