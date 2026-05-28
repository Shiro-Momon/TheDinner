using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using RestaurantOrder.Application.DTOs.Menu;
using RestaurantOrder.Application.DTOs.Orders;
using RestaurantOrder.Application.DTOs.Payments;
using RestaurantOrder.Application.DTOs.Tables;
using RestaurantOrder.Domain.Enums;

namespace RestaurantOrder.IntegrationTests;

public class PaymentEndpointsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PaymentEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Should_ProcessPayment_When_OrderIsServed()
    {
        var order = await CreateServedOrderAsync();

        var dto = new CreatePaymentDto(order.Id, PaymentMethod.Cash, TipAmount: 2m);
        var response = await _client.PostAsJsonAsync("/api/payments", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payment = await response.Content.ReadFromJsonAsync<PaymentResponseDto>(TestJsonOptions.Default);
        payment!.OrderId.Should().Be(order.Id);
        payment.Method.Should().Be(PaymentMethod.Cash);
        payment.TipAmount.Should().Be(2m);
        payment.TransactionReference.Should().StartWith("CASH-");
    }

    [Fact]
    public async Task Should_ReturnPayment_When_Exists()
    {
        var order = await CreateServedOrderAsync();
        var created = await ProcessPaymentAsync(order.Id, PaymentMethod.CreditCard);

        var response = await _client.GetAsync($"/api/payments/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payment = await response.Content.ReadFromJsonAsync<PaymentResponseDto>(TestJsonOptions.Default);
        payment!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Should_GetPaymentByOrderId_When_Paid()
    {
        var order = await CreateServedOrderAsync();
        await ProcessPaymentAsync(order.Id, PaymentMethod.MealVoucher);

        var response = await _client.GetAsync($"/api/payments/order/{order.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var payment = await response.Content.ReadFromJsonAsync<PaymentResponseDto>(TestJsonOptions.Default);
        payment!.OrderId.Should().Be(order.Id);
    }

    [Fact]
    public async Task Should_Return422_When_OrderAlreadyPaid()
    {
        var order = await CreateServedOrderAsync();
        await ProcessPaymentAsync(order.Id, PaymentMethod.Cash);

        var response = await _client.PostAsJsonAsync("/api/payments",
            new CreatePaymentDto(order.Id, PaymentMethod.Cash));

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task Should_Return422_When_PaymentNotFound()
    {
        var response = await _client.GetAsync("/api/payments/99999");

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    private async Task<OrderResponseDto> CreateServedOrderAsync()
    {
        var tableNumber = Random.Shared.Next(10_000, 99_999);
        var tableResponse = await _client.PostAsJsonAsync("/api/tables", new CreateTableDto(tableNumber, 4));
        var table = (await tableResponse.Content.ReadFromJsonAsync<TableResponseDto>(TestJsonOptions.Default))!;

        var itemResponse = await _client.PostAsJsonAsync("/api/menu",
            new CreateMenuItemDto("Payment Test Item", 15m, MenuItemCategory.MainCourse));
        var item = (await itemResponse.Content.ReadFromJsonAsync<MenuItemResponseDto>(TestJsonOptions.Default))!;

        var orderResponse = await _client.PostAsJsonAsync("/api/orders",
            new CreateOrderDto(table.Id, false, PricingStrategyType.Standard, new List<CreateOrderItemDto> { new(item.Id, 1) }));
        var order = (await orderResponse.Content.ReadFromJsonAsync<OrderResponseDto>(TestJsonOptions.Default))!;

        await _client.PatchAsync($"/api/orders/{order.Id}/confirm", null);
        await _client.PatchAsync($"/api/orders/{order.Id}/prepare", null);
        await _client.PatchAsync($"/api/orders/{order.Id}/ready", null);
        await _client.PatchAsync($"/api/orders/{order.Id}/serve", null);

        return order;
    }

    private async Task<PaymentResponseDto> ProcessPaymentAsync(int orderId, PaymentMethod method)
    {
        var response = await _client.PostAsJsonAsync("/api/payments",
            new CreatePaymentDto(orderId, method));
        return (await response.Content.ReadFromJsonAsync<PaymentResponseDto>(TestJsonOptions.Default))!;
    }
}
