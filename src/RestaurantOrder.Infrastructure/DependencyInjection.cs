using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Infrastructure.EventDispatching;
using RestaurantOrder.Infrastructure.PaymentProcessors;
using RestaurantOrder.Infrastructure.Persistence;
using RestaurantOrder.Infrastructure.Repositories;
using RestaurantOrder.Infrastructure.Strategies;

namespace RestaurantOrder.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<RestaurantDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        services.AddScoped<IPricingStrategy, StandardPricingStrategy>();

        services.AddScoped<IPaymentProcessor, CashPaymentProcessor>();
        services.AddScoped<IPaymentProcessor, CreditCardPaymentProcessor>();
        services.AddScoped<IPaymentProcessor, MealVoucherPaymentProcessor>();

        services.AddScoped<DomainEventDispatcher>();

        return services;
    }
}
