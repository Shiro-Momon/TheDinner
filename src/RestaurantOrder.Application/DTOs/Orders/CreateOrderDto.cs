namespace RestaurantOrder.Application.DTOs.Orders;

public record CreateOrderDto(
    Guid TableId,
    List<CreateOrderItemDto> Items);
