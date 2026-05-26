using FluentAssertions;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.UnitTests.Domain;

public class ExceptionTests
{
    [Fact]
    public void Should_CreateDomainException_With_InnerException()
    {
        var inner = new Exception("inner");

        var ex = new DomainException("outer", inner);

        ex.Message.Should().Be("outer");
        ex.InnerException.Should().Be(inner);
    }

    [Fact]
    public void Should_CreateInsufficientPaymentException_With_FormattedMessage()
    {
        var ex = new InsufficientPaymentException(100m, 80m);

        ex.Message.Should().Contain("80");
        ex.Message.Should().Contain("100");
    }
}
