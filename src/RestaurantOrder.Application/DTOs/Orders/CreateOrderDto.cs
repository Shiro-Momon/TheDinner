namespace RestaurantOrder.Application.DTOs.Orders;

public record CreateOrderDto(
    int TableId,
    List<CreateOrderItemDto> Items);
