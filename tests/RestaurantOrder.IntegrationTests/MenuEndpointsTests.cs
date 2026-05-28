using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RestaurantOrder.Application.DTOs.Menu;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.IntegrationTests;

public class MenuEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MenuEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_ReturnOkWithList_When_GetAllMenu()
    {
        var response = await _client.GetAsync("/api/menu");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemResponseDto>>(TestJsonOptions.Default);
        items.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_CreateMenuItem_When_ValidDto()
    {
        var dto = new CreateMenuItemDto("Burger", 12.50m, MenuItemCategory.MainCourse);

        var response = await _client.PostAsJsonAsync("/api/menu", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var item = await response.Content.ReadFromJsonAsync<MenuItemResponseDto>(TestJsonOptions.Default);
        item!.Name.Should().Be("Burger");
        item.Price.Should().Be(12.50m);
    }

    [Fact]
    public async Task Should_ReturnMenuItem_When_Exists()
    {
        var created = await CreateMenuItemAsync("Salad", 8m, MenuItemCategory.Starter);

        var response = await _client.GetAsync($"/api/menu/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var item = await response.Content.ReadFromJsonAsync<MenuItemResponseDto>(TestJsonOptions.Default);
        item!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Should_Return422_When_MenuItemNotFound()
    {
        var response = await _client.GetAsync("/api/menu/999999");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_UpdateMenuItem_When_ValidDto()
    {
        var created = await CreateMenuItemAsync("OldName", 5m, MenuItemCategory.Side);
        var updateDto = new UpdateMenuItemDto("NewName", 7m, MenuItemCategory.Side, false);

        var response = await _client.PutAsJsonAsync($"/api/menu/{created.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<MenuItemResponseDto>(TestJsonOptions.Default);
        updated!.Name.Should().Be("NewName");
        updated.IsAvailable.Should().BeFalse();
    }

    [Fact]
    public async Task Should_DeleteMenuItem_When_Exists()
    {
        var created = await CreateMenuItemAsync("ToDelete", 3m, MenuItemCategory.Beverage);

        var response = await _client.DeleteAsync($"/api/menu/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Should_FilterByCategory_When_CategoryProvided()
    {
        await CreateMenuItemAsync("Espresso", 2m, MenuItemCategory.Beverage);
        await CreateMenuItemAsync("Latte", 3m, MenuItemCategory.Beverage);
        await CreateMenuItemAsync("Steak", 25m, MenuItemCategory.MainCourse);

        var response = await _client.GetAsync($"/api/menu/category/{MenuItemCategory.Beverage}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await response.Content.ReadFromJsonAsync<List<MenuItemResponseDto>>(TestJsonOptions.Default);
        items!.Should().OnlyContain(i => i.Category == MenuItemCategory.Beverage);
    }

    private async Task<MenuItemResponseDto> CreateMenuItemAsync(string name, decimal price, MenuItemCategory category)
    {
        var response = await _client.PostAsJsonAsync("/api/menu", new CreateMenuItemDto(name, price, category));
        return (await response.Content.ReadFromJsonAsync<MenuItemResponseDto>(TestJsonOptions.Default))!;
    }
}
