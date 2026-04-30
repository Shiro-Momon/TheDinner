using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Domain.Entities;

public class Payment
{
    private Payment()
    {
    }

    public Guid Id { get; private set; }
    public Guid OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal TipAmount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string TransactionReference { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; private set; }

    public static Payment Create(
        Guid orderId,
        decimal amount,
        decimal tipAmount,
        PaymentMethod method,
        string transactionReference)
    {
        return new Payment
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            Amount = amount,
            TipAmount = tipAmount,
            Method = method,
            TransactionReference = transactionReference,
            ProcessedAt = DateTimeOffset.UtcNow,
        };
    }
}
