using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Domain.Entities;

public class Payment
{
    private Payment()
    {
    }

    public int Id { get; private set; }
    public int OrderId { get; private set; }
    public decimal Amount { get; private set; }
    public decimal TipAmount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string TransactionReference { get; private set; } = string.Empty;
    public DateTimeOffset ProcessedAt { get; private set; }

    public static Payment Create(
        int orderId,
        decimal amount,
        decimal tipAmount,
        PaymentMethod method,
        string transactionReference)
    {
        return new Payment
        {
            OrderId = orderId,
            Amount = amount,
            TipAmount = tipAmount,
            Method = method,
            TransactionReference = transactionReference,
            ProcessedAt = DateTimeOffset.UtcNow,
        };
    }
}
