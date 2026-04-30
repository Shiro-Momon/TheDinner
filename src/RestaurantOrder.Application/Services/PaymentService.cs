using RestaurantOrder.Application.DTOs.Payments;
using RestaurantOrder.Application.Interfaces.Repositories;
using RestaurantOrder.Domain.Entities;
using RestaurantOrder.Domain.Exceptions;

namespace RestaurantOrder.Application.Services;

public class PaymentService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly PaymentFactory _paymentFactory;

    public PaymentService(
        IOrderRepository orderRepository,
        IPaymentRepository paymentRepository,
        PaymentFactory paymentFactory)
    {
        _orderRepository = orderRepository;
        _paymentRepository = paymentRepository;
        _paymentFactory = paymentFactory;
    }

    public async Task<PaymentResponseDto> ProcessPaymentAsync(CreatePaymentDto dto, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetWithItemsAsync(dto.OrderId, ct)
            ?? throw new DomainException($"Order '{dto.OrderId}' not found.");

        var existing = await _paymentRepository.GetByOrderIdAsync(dto.OrderId, ct);
        if (existing is not null)
            throw new DomainException($"Order '{dto.OrderId}' has already been paid.");

        var processor = _paymentFactory.GetProcessor(dto.Method);
        var transactionRef = await processor.ProcessAsync(order.TotalAmount + dto.TipAmount, ct);

        var payment = Payment.Create(order.Id, order.TotalAmount, dto.TipAmount, dto.Method, transactionRef);
        await _paymentRepository.AddAsync(payment, ct);

        order.MarkPaid(dto.Method);
        await _orderRepository.UpdateAsync(order, ct);

        return MapToDto(payment);
    }

    public async Task<PaymentResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, ct)
            ?? throw new DomainException($"Payment '{id}' not found.");
        return MapToDto(payment);
    }

    public async Task<PaymentResponseDto> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var payment = await _paymentRepository.GetByOrderIdAsync(orderId, ct)
            ?? throw new DomainException($"No payment found for order '{orderId}'.");
        return MapToDto(payment);
    }

    private static PaymentResponseDto MapToDto(Payment payment) =>
        new(
            payment.Id,
            payment.OrderId,
            payment.Amount,
            payment.TipAmount,
            payment.Method,
            payment.TransactionReference,
            payment.ProcessedAt);
}
