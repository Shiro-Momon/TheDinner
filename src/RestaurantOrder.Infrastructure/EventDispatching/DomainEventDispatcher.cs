using Microsoft.Extensions.DependencyInjection;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Events;

namespace RestaurantOrder.Infrastructure.EventDispatching;

public class DomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public DomainEventDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task DispatchAsync(Order order, CancellationToken ct = default)
    {
        var events = order.DomainEvents.ToList();
        order.ClearDomainEvents();

        foreach (var domainEvent in events)
        {
            await DispatchEventAsync(domainEvent, ct);
        }
    }

    private async Task DispatchEventAsync(IDomainEvent domainEvent, CancellationToken ct)
    {
        var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
        var handlers = _serviceProvider.GetServices(handlerType);

        foreach (var handler in handlers)
        {
            var method = handlerType.GetMethod(nameof(IDomainEventHandler<IDomainEvent>.HandleAsync))!;
            await (Task)method.Invoke(handler, new object[] { domainEvent, ct })!;
        }
    }
}
