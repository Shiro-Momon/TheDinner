using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.Application.DTOs.Orders;

public record CreateOrderDto(
    int? TableId,
    bool IsToGo,
    PricingStrategyType PricingStrategy,
    List<CreateOrderItemDto> Items);
