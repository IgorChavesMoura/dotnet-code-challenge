using WebApp.Data;
using WebApp.Models;
using WebApp.PaymentGateways;

namespace WebApp.Services;

public class PaymentService
{
    private readonly AppDbContext _context;
    private readonly PaymentGatewayFactory _gatewayFactory;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(AppDbContext context, PaymentGatewayFactory gatewayFactory, ILogger<PaymentService> logger)
    {
        _context = context;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    public async Task<Payment> ProcessPaymentAsync(string orderId, decimal amount)
    {
        var gateway = _gatewayFactory.GetPaymentGateway(orderId);
        var result = await gateway.ProcessPaymentAsync(orderId, amount);

        var payment = new Payment
        {
            PaymentId = $"PAY-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            OrderId = orderId,
            Amount = amount,
            Status = result.Success ? "Completed" : "Failed",
            ProcessedAt = DateTime.UtcNow
        };

        await _context.Payments.InsertOneAsync(payment);

        _logger.LogInformation("Payment {PaymentId} processed for order {OrderId}", payment.PaymentId, orderId);

        return payment;
    }

    public async Task PublishPaymentEventAsync(Payment payment)
    {
        var paymentEvent = new
        {
            payment.PaymentId,
            payment.OrderId,
            payment.Amount,
            payment.Status,
            payment.ProcessedAt
        };

        _logger.LogInformation("Publishing payment event for {PaymentId}", payment.PaymentId);
    }
}
