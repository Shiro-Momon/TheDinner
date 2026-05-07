namespace RestaurantOrder.Application.DTOs.Tables;

public record TableResponseDto(
    int Id,
    int Number,
    int Capacity,
    bool IsOccupied);
