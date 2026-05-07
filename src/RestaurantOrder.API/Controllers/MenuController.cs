using Microsoft.AspNetCore.Mvc;
using RestaurantOrder.Application.DTOs.Menu;
using RestaurantOrder.Application.Services;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.API.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly MenuService _menuService;

    public MenuController(MenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(await _menuService.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct) =>
        Ok(await _menuService.GetByIdAsync(id, ct));

    [HttpGet("category/{category}")]
    public async Task<IActionResult> GetByCategory(MenuItemCategory category, CancellationToken ct) =>
        Ok(await _menuService.GetByCategoryAsync(category, ct));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMenuItemDto dto, CancellationToken ct)
    {
        var result = await _menuService.CreateAsync(dto, ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuItemDto dto, CancellationToken ct) =>
        Ok(await _menuService.UpdateAsync(id, dto, ct));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _menuService.DeleteAsync(id, ct);
        return NoContent();
    }
}
