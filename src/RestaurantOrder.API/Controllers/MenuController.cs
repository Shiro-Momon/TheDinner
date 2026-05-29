using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
    private readonly IWebHostEnvironment _env;

    public MenuController(MenuService menuService, IWebHostEnvironment env)
    {
        _menuService = menuService;
        _env = env;
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

    [HttpPost("images")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { error = "No file uploaded." });

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        var ext = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(ext))
            return BadRequest(new { error = "Invalid image format. Allowed: .jpg, .jpeg, .png, .webp, .gif" });

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest(new { error = "File size exceeds the 5MB limit." });

        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var imagesDir = Path.Combine(webRoot, "images");
        if (!Directory.Exists(imagesDir))
            Directory.CreateDirectory(imagesDir);

        var fileName = $"{Guid.NewGuid()}{ext}";
        var filePath = Path.Combine(imagesDir, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var relativePath = $"/images/{fileName}";
        return Ok(new { imageUrl = relativePath });
    }
}
