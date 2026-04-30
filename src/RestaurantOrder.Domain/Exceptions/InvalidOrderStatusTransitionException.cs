using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Domain.Exceptions;

public class InvalidOrderStatusTransitionException : DomainException
{
    public InvalidOrderStatusTransitionException(OrderStatus current, OrderStatus expected)
        : base($"Cannot transition order from '{current}' to '{expected}'.")
    {
    }
}
