namespace WebApp.PaymentGateways;

public class PayPalPaymentGateway : IPaymentGateway
{
    private readonly ILogger<PayPalPaymentGateway> _logger;

    public PayPalPaymentGateway(ILogger<PayPalPaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing PayPal payment for order {OrderId}, amount {Amount}", orderId, amount);
        
        await Task.Delay(150, cancellationToken);
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"PAYID-{Guid.NewGuid().ToString()[..12].ToUpper()}"
        };
    }

    public async Task<PaymentResult> RefundPaymentAsync(string paymentId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing PayPal refund for payment {PaymentId}, amount {Amount}", paymentId, amount);
        
        await Task.Delay(150, cancellationToken);
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"REFID-{Guid.NewGuid().ToString()[..12].ToUpper()}"
        };
    }
}
