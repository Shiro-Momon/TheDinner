using RestaurantOrder.Application.DTOs.Tables;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Application.Services;

public class TableService
{
    private readonly ITableRepository _repository;

    public TableService(ITableRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<TableResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var tables = await _repository.GetAllAsync(ct);
        return tables.Select(MapToDto).ToList();
    }

    public async Task<TableResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var table = await _repository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Table '{id}' not found.");
        return MapToDto(table);
    }

    public async Task<TableResponseDto> CreateAsync(CreateTableDto dto, CancellationToken ct = default)
    {
        var table = Table.Create(dto.Number, dto.Capacity);
        await _repository.AddAsync(table, ct);
        return MapToDto(table);
    }

    public async Task<TableResponseDto> OccupyAsync(Guid id, CancellationToken ct = default)
    {
        var table = await _repository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Table '{id}' not found.");

        table.Occupy();
        await _repository.UpdateAsync(table, ct);
        return MapToDto(table);
    }

    public async Task<TableResponseDto> ReleaseAsync(Guid id, CancellationToken ct = default)
    {
        var table = await _repository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Table '{id}' not found.");

        table.Release();
        await _repository.UpdateAsync(table, ct);
        return MapToDto(table);
    }

    private static TableResponseDto MapToDto(Table table) =>
        new(table.Id, table.Number, table.Capacity, table.IsOccupied);
}
