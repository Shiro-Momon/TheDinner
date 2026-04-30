using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Payments;

public record CreatePaymentDto(
    Guid OrderId,
    PaymentMethod Method,
    decimal TipAmount = 0);
