using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Payments;

public record PaymentResponseDto(
    Guid Id,
    Guid OrderId,
    decimal Amount,
    decimal TipAmount,
    PaymentMethod Method,
    string TransactionReference,
    DateTimeOffset ProcessedAt);
