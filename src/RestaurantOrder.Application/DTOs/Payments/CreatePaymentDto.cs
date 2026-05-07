using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Payments;

public record CreatePaymentDto(
    int OrderId,
    PaymentMethod Method,
    decimal TipAmount = 0);
