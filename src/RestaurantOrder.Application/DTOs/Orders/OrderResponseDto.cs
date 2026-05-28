using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Orders;

public record OrderResponseDto(
    int Id,
    int? TableId,
    bool IsToGo,
    PricingStrategyType PricingStrategy,
    string? CustomerName,
    OrderStatus Status,
    List<OrderItemResponseDto> Items,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ConfirmedAt,
    DateTimeOffset? ServedAt,
    DateTimeOffset? PaidAt);
