using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Orders;

public record OrderResponseDto(
    Guid Id,
    Guid TableId,
    OrderStatus Status,
    List<OrderItemResponseDto> Items,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    DateTimeOffset? ConfirmedAt,
    DateTimeOffset? ServedAt,
    DateTimeOffset? PaidAt);
