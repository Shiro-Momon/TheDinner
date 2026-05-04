using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RestaurantOrder.Application.DTOs.Menu;
using RestaurantOrder.Application.DTOs.Orders;
using RestaurantOrder.Application.DTOs.Tables;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.IntegrationTests;

public class OrderEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public OrderEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_CreateOrder_When_ValidDto()
    {
        var table = await CreateTableAsync();
        var menuItem = await CreateMenuItemAsync();

        var dto = new CreateOrderDto(table.Id, new List<CreateOrderItemDto>
        {
            new(menuItem.Id, 2),
        });

        var response = await _client.PostAsJsonAsync("/api/orders", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var order = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        order!.TableId.Should().Be(table.Id);
        order.Status.Should().Be(OrderStatus.Pending);
        order.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Should_ConfirmOrder_When_OrderIsPending()
    {
        var order = await CreateOrderAsync();

        var response = await _client.PatchAsync($"/api/orders/{order.Id}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        updated!.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public async Task Should_Return422_When_ConfirmingAlreadyConfirmedOrder()
    {
        var order = await CreateOrderAsync();
        await _client.PatchAsync($"/api/orders/{order.Id}/confirm", null);

        var response = await _client.PatchAsync($"/api/orders/{order.Id}/confirm", null);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_FollowFullLifecycle_When_ValidTransitions()
    {
        var order = await CreateOrderAsync();
        var id = order.Id;

        (await _client.PatchAsync($"/api/orders/{id}/confirm", null)).StatusCode
            .Should().Be(HttpStatusCode.OK);
        (await _client.PatchAsync($"/api/orders/{id}/prepare", null)).StatusCode
            .Should().Be(HttpStatusCode.OK);
        (await _client.PatchAsync($"/api/orders/{id}/ready", null)).StatusCode
            .Should().Be(HttpStatusCode.OK);
        (await _client.PatchAsync($"/api/orders/{id}/serve", null)).StatusCode
            .Should().Be(HttpStatusCode.OK);

        var response = await _client.GetAsync($"/api/orders/{id}");
        var final = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        final!.Status.Should().Be(OrderStatus.Served);
    }

    [Fact]
    public async Task Should_CancelOrder_When_NotPaid()
    {
        var order = await CreateOrderAsync();

        var response = await _client.PatchAsync($"/api/orders/{order.Id}/cancel", null);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var updated = await response.Content.ReadFromJsonAsync<OrderResponseDto>();
        updated!.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public async Task Should_Return422_When_OrderNotFound()
    {
        var response = await _client.GetAsync($"/api/orders/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private async Task<TableResponseDto> CreateTableAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/tables", new CreateTableDto(99, 4));
        return (await response.Content.ReadFromJsonAsync<TableResponseDto>())!;
    }

    private async Task<MenuItemResponseDto> CreateMenuItemAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/menu", new CreateMenuItemDto("Test Item", 10m, MenuItemCategory.MainCourse));
        return (await response.Content.ReadFromJsonAsync<MenuItemResponseDto>())!;
    }

    private async Task<OrderResponseDto> CreateOrderAsync()
    {
        var table = await CreateTableAsync();
        var menuItem = await CreateMenuItemAsync();
        var dto = new CreateOrderDto(table.Id, new List<CreateOrderItemDto> { new(menuItem.Id, 1) });
        var response = await _client.PostAsJsonAsync("/api/orders", dto);
        return (await response.Content.ReadFromJsonAsync<OrderResponseDto>())!;
    }
}
