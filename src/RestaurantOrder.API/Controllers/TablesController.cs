using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.Application.DTOs.Tables;
using RestaurantOrder.Application.Services;

namespace RestaurantOrder.API.Controllers;

[ApiController]
[Route("api/tables")]
public class TablesController : ControllerBase
{
    private readonly TableService _tableService;

    public TablesController(TableService tableService)
    {
        _tableService = tableService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _tableService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await _tableService.GetByIdAsync(id, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTableDto dto, CancellationToken ct)
    {
        var result = await _tableService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPatch("{id:int}/occupy")]
    public async Task<IActionResult> Occupy(int id, CancellationToken ct) =>
        Ok(await _tableService.OccupyAsync(id, ct));

    [HttpPatch("{id:int}/release")]
    public async Task<IActionResult> Release(int id, CancellationToken ct) =>
        Ok(await _tableService.ReleaseAsync(id, ct));
}
