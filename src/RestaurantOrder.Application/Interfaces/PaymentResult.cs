namespace RestaurantOrder.Application.Interfaces;

public sealed record PaymentResult
{
    public bool IsSuccess { get; init; }
    public string? TransactionReference { get; init; }
    public string? FailureReason { get; init; }

    public static PaymentResult Success(string transactionReference) =>
        new() { IsSuccess = true, TransactionReference = transactionReference };

    public static PaymentResult Declined(string reason) =>
        new() { IsSuccess = false, FailureReason = reason };
}
