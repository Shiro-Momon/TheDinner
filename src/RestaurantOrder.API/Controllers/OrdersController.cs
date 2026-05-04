using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.Application.DTOs.Orders;
using RestaurantOrder.Application.Services;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.API.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;

    public OrdersController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] OrderStatus? status, CancellationToken ct) =>
        Ok(await _orderService.GetAllAsync(status, ct));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(await _orderService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:guid}/items")]
    public async Task<IActionResult> AddItem(Guid id, [FromBody] AddOrderItemDto dto, CancellationToken ct) =>
        Ok(await _orderService.AddItemAsync(id, dto, ct));

    [HttpDelete("{id:guid}/items/{menuItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid id, Guid menuItemId, CancellationToken ct) =>
        Ok(await _orderService.RemoveItemAsync(id, menuItemId, ct));

    [HttpPatch("{id:guid}/confirm")]
    public async Task<IActionResult> Confirm(Guid id, CancellationToken ct) =>
        Ok(await _orderService.ConfirmAsync(id, ct));

    [HttpPatch("{id:guid}/prepare")]
    public async Task<IActionResult> Prepare(Guid id, CancellationToken ct) =>
        Ok(await _orderService.StartPreparingAsync(id, ct));

    [HttpPatch("{id:guid}/ready")]
    public async Task<IActionResult> MarkReady(Guid id, CancellationToken ct) =>
        Ok(await _orderService.MarkReadyAsync(id, ct));

    [HttpPatch("{id:guid}/serve")]
    public async Task<IActionResult> Serve(Guid id, CancellationToken ct) =>
        Ok(await _orderService.ServeAsync(id, ct));

    [HttpPatch("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct) =>
        Ok(await _orderService.CancelAsync(id, ct));
}
