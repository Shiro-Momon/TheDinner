namespace RestaurantOrder.Domain.Exceptions;

public class InsufficientPaymentException : DomainException
{
    public InsufficientPaymentException(decimal required, decimal provided)
        : base($"Payment of {provided:C} is insufficient. Required: {required:C}.")
    {
    }
}
