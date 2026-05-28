using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RestaurantOrder.Application.DTOs.Tables;

namespace RestaurantOrder.IntegrationTests;

public class TableEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TableEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_ReturnOkWithList_When_GetAllTables()
    {
        var response = await _client.GetAsync("/api/tables");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var tables = await response.Content.ReadFromJsonAsync<List<TableResponseDto>>(TestJsonOptions.Default);
        tables.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_CreateTable_When_ValidDto()
    {
        var response = await _client.PostAsJsonAsync("/api/tables", new CreateTableDto(1, 4));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var table = await response.Content.ReadFromJsonAsync<TableResponseDto>(TestJsonOptions.Default);
        table!.Number.Should().Be(1);
        table.Capacity.Should().Be(4);
        table.IsOccupied.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnTable_When_Exists()
    {
        var created = await CreateTableAsync(2, 6);

        var response = await _client.GetAsync($"/api/tables/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var table = await response.Content.ReadFromJsonAsync<TableResponseDto>(TestJsonOptions.Default);
        table!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Should_Return422_When_TableNotFound()
    {
        var response = await _client.GetAsync("/api/tables/999999");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_Return422_When_OccupyTableNotFound()
    {
        var response = await _client.PatchAsync("/api/tables/999999/occupy", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_Return422_When_ReleaseTableNotFound()
    {
        var response = await _client.PatchAsync("/api/tables/999999/release", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_OccupyTable_When_TableIsFree()
    {
        var created = await CreateTableAsync(3, 2);

        var response = await _client.PatchAsync($"/api/tables/{created.Id}/occupy", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var table = await response.Content.ReadFromJsonAsync<TableResponseDto>(TestJsonOptions.Default);
        table!.IsOccupied.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReleaseTable_When_TableIsOccupied()
    {
        var created = await CreateTableAsync(4, 4);
        await _client.PatchAsync($"/api/tables/{created.Id}/occupy", null);

        var response = await _client.PatchAsync($"/api/tables/{created.Id}/release", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var table = await response.Content.ReadFromJsonAsync<TableResponseDto>(TestJsonOptions.Default);
        table!.IsOccupied.Should().BeFalse();
    }

    private async Task<TableResponseDto> CreateTableAsync(int number, int capacity)
    {
        var response = await _client.PostAsJsonAsync("/api/tables", new CreateTableDto(number, capacity));
        return (await response.Content.ReadFromJsonAsync<TableResponseDto>(TestJsonOptions.Default))!;
    }
}
