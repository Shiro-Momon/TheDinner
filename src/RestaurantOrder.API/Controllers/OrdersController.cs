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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await _orderService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var result = await _orderService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPost("{id:int}/items")]
    public async Task<IActionResult> AddItem(int id, [FromBody] AddOrderItemDto dto, CancellationToken ct) =>
        Ok(await _orderService.AddItemAsync(id, dto, ct));

    [HttpDelete("{id:int}/items/{menuItemId:int}")]
    public async Task<IActionResult> RemoveItem(int id, int menuItemId, CancellationToken ct) =>
        Ok(await _orderService.RemoveItemAsync(id, menuItemId, ct));

    [HttpPatch("{id:int}/confirm")]
    public async Task<IActionResult> Confirm(int id, CancellationToken ct) =>
        Ok(await _orderService.ConfirmAsync(id, ct));

    [HttpPatch("{id:int}/prepare")]
    public async Task<IActionResult> Prepare(int id, CancellationToken ct) =>
        Ok(await _orderService.StartPreparingAsync(id, ct));

    [HttpPatch("{id:int}/ready")]
    public async Task<IActionResult> MarkReady(int id, CancellationToken ct) =>
        Ok(await _orderService.MarkReadyAsync(id, ct));

    [HttpPatch("{id:int}/serve")]
    public async Task<IActionResult> Serve(int id, CancellationToken ct) =>
        Ok(await _orderService.ServeAsync(id, ct));

    [HttpPatch("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id, CancellationToken ct) =>
        Ok(await _orderService.CancelAsync(id, ct));
}
