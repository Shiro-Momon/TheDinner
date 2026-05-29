using RestaurantOrder.Application.DTOs.Menu;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Enums;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Application.Services;

public class MenuService
{
    private readonly IMenuItemRepository _repository;

    public MenuService(IMenuItemRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<MenuItemResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repository.GetAllAsync(ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<MenuItemResponseDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Menu item '{id}' not found.");
        return MapToDto(item);
    }

    public async Task<IReadOnlyList<MenuItemResponseDto>> GetByCategoryAsync(MenuItemCategory category, CancellationToken ct = default)
    {
        var items = await _repository.GetByCategoryAsync(category, ct);
        return items.Select(MapToDto).ToList();
    }

    public async Task<MenuItemResponseDto> CreateAsync(CreateMenuItemDto dto, CancellationToken ct = default)
    {
        var item = MenuItem.Create(dto.Name, dto.Price, dto.Category, dto.IsAvailable, dto.ImageUrl);
        await _repository.AddAsync(item, ct);
        return MapToDto(item);
    }

    public async Task<MenuItemResponseDto> UpdateAsync(int id, UpdateMenuItemDto dto, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Menu item '{id}' not found.");

        item.Update(dto.Name, dto.Price, dto.Category, dto.IsAvailable, dto.ImageUrl);
        await _repository.UpdateAsync(item, ct);
        return MapToDto(item);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var item = await _repository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Menu item '{id}' not found.");
        await _repository.DeleteAsync(item.Id, ct);
    }

    private static MenuItemResponseDto MapToDto(MenuItem item) =>
        new(item.Id, item.Name, item.Price, item.Category, item.IsAvailable, item.ImageUrl);
}
