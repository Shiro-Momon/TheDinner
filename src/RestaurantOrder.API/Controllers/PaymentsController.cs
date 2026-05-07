using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.Application.DTOs.Payments;
using RestaurantOrder.Application.Services;

namespace RestaurantOrder.API.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost]
    public async Task<IActionResult> Process([FromBody] CreatePaymentDto dto, CancellationToken ct)
    {
        var result = await _paymentService.ProcessPaymentAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await _paymentService.GetByIdAsync(id, ct));

    [HttpGet("order/{orderId:int}")]
    public async Task<IActionResult> GetByOrderId(int orderId, CancellationToken ct) =>
        Ok(await _paymentService.GetByOrderIdAsync(orderId, ct));
}
