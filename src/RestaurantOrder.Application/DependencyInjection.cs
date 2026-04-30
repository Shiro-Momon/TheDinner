using Microsoft.Extensions.DependencyInjection;
using RestaurantOrder.Application.EventHandlers;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Application.Services;
using RestaurantOrder.Domain.Events;

namespace RestaurantOrder.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<MenuService>();
        services.AddScoped<TableService>();
        services.AddScoped<OrderService>();
        services.AddScoped<PaymentService>();
        services.AddScoped<PaymentFactory>();

        services.AddScoped<IDomainEventHandler<OrderConfirmedEvent>, OrderConfirmedEventHandler>();
        services.AddScoped<IDomainEventHandler<OrderPaidEvent>, OrderPaidEventHandler>();

        return services;
    }
}
