using RestaurantOrder.Application.DTOs.Orders;
using RestaurantOrder.Application.Interfaces;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemRepository _menuItemRepository;
    private readonly ITableRepository _tableRepository;
    private readonly IPricingStrategyFactory _pricingStrategyFactory;

    public OrderService(
        IOrderRepository orderRepository,
        IMenuItemRepository menuItemRepository,
        ITableRepository tableRepository,
        IPricingStrategyFactory pricingStrategyFactory)
    {
        _orderRepository = orderRepository;
        _menuItemRepository = menuItemRepository;
        _tableRepository = tableRepository;
        _pricingStrategyFactory = pricingStrategyFactory;
    }

    public async Task<IReadOnlyList<OrderResponseDto>> GetAllAsync(OrderStatus? status = null, CancellationToken ct = default)
    {
        var orders = status.HasValue
            ? await _orderRepository.GetByStatusAsync(status.Value, ct)
            : await _orderRepository.GetAllAsync(ct);

        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderResponseDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(id, ct)
            ?? throw new DomainException($"Order '{id}' not found.");
        return MapToDto(order);
    }

    public async Task<OrderResponseDto> CreateAsync(CreateOrderDto dto, CancellationToken ct = default)
    {
        var order = Order.Create(dto.TableId, dto.IsToGo, dto.PricingStrategy, dto.CustomerName);

        foreach (var itemDto in dto.Items)
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(itemDto.MenuItemId, ct)
                ?? throw new DomainException($"Menu item '{itemDto.MenuItemId}' not found.");

            if (!menuItem.IsAvailable)
                throw new DomainException($"Menu item '{menuItem.Name}' is not available.");

            order.AddItem(OrderItem.Create(menuItem.Id, itemDto.Quantity, menuItem.Price, itemDto.SpecialInstructions));
        }

        var strategy = _pricingStrategyFactory.GetStrategy(dto.PricingStrategy);
        order.SetFinalTotal(strategy.CalculateTotal(order.Items));

        await _orderRepository.AddAsync(order, ct);

        if (!dto.IsToGo && dto.TableId.HasValue)
        {
            var table = await _tableRepository.GetByIdAsync(dto.TableId.Value, ct)
                ?? throw new DomainException($"Table '{dto.TableId}' not found.");
            table.Occupy();
            await _tableRepository.UpdateAsync(table, ct);
        }

        return MapToDto(order);
    }

    public async Task<OrderResponseDto> AddItemAsync(int orderId, AddOrderItemDto dto, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(orderId, ct)
            ?? throw new DomainException($"Order '{orderId}' not found.");

        var menuItem = await _menuItemRepository.GetByIdAsync(dto.MenuItemId, ct)
            ?? throw new DomainException($"Menu item '{dto.MenuItemId}' not found.");

        if (!menuItem.IsAvailable)
            throw new DomainException($"Menu item '{menuItem.Name}' is not available.");

        order.AddItem(OrderItem.Create(menuItem.Id, dto.Quantity, menuItem.Price, dto.SpecialInstructions));
        await _orderRepository.UpdateAsync(order, ct);
        return MapToDto(order);
    }

    public async Task<OrderResponseDto> RemoveItemAsync(int orderId, int menuItemId, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(orderId, ct)
            ?? throw new DomainException($"Order '{orderId}' not found.");

        order.RemoveItem(menuItemId);
        await _orderRepository.UpdateAsync(order, ct);
        return MapToDto(order);
    }

    public async Task<OrderResponseDto> ConfirmAsync(int id, CancellationToken ct = default) =>
        await TransitionAsync(id, o => o.Confirm(), ct);

    public async Task<OrderResponseDto> StartPreparingAsync(int id, CancellationToken ct = default) =>
        await TransitionAsync(id, o => o.StartPreparing(), ct);

    public async Task<OrderResponseDto> MarkReadyAsync(int id, CancellationToken ct = default) =>
        await TransitionAsync(id, o => o.MarkReady(), ct);

    public async Task<OrderResponseDto> ServeAsync(int id, CancellationToken ct = default) =>
        await TransitionAsync(id, o => o.Serve(), ct);

    public async Task<OrderResponseDto> CancelAsync(int id, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(id, ct)
            ?? throw new DomainException($"Order '{id}' not found.");

        order.Cancel();
        await _orderRepository.UpdateAsync(order, ct);

        if (!order.IsToGo && order.TableId.HasValue)
        {
            var table = await _tableRepository.GetByIdAsync(order.TableId.Value, ct);
            if (table is not null)
            {
                table.Release();
                await _tableRepository.UpdateAsync(table, ct);
            }
        }

        return MapToDto(order);
    }

    private async Task<OrderResponseDto> TransitionAsync(int id, Action<Order> transition, CancellationToken ct)
    {
        var order = await _orderRepository.GetWithItemsAsync(id, ct)
            ?? throw new DomainException($"Order '{id}' not found.");

        transition(order);
        await _orderRepository.UpdateAsync(order, ct);
        return MapToDto(order);
    }

    private static OrderResponseDto MapToDto(Order order) =>
        new(
            order.Id,
            order.TableId,
            order.IsToGo,
            order.PricingStrategy,
            order.CustomerName,
            order.Status,
            order.Items.Select(i => new OrderItemResponseDto(
                i.Id, i.MenuItemId, i.Quantity, i.UnitPrice, i.SubTotal, i.SpecialInstructions)).ToList(),
            order.TotalAmount,
            order.CreatedAt,
            order.ConfirmedAt,
            order.ServedAt,
            order.PaidAt);
}
