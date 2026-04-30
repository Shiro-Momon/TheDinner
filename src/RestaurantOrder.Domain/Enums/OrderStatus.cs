namespace RestaurantOrder.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,
    Confirmed,
    Preparing,
    Ready,
    Served,
    Paid,
    Cancelled,
}
