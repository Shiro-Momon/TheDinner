namespace RestaurantOrder.Application.DTOs.Tables;

public record TableResponseDto(
    Guid Id,
    int Number,
    int Capacity,
    bool IsOccupied);
