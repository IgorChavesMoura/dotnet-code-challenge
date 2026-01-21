namespace WebApp.PaymentGateways;

public sealed class StripePaymentGateway : IPaymentGateway
{
    private readonly ILogger<StripePaymentGateway> _logger;

    public StripePaymentGateway(ILogger<StripePaymentGateway> logger)
    {
        _logger = logger;
    }

    public async Task<PaymentResult> ProcessPaymentAsync(string orderId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing Stripe payment for order {OrderId}, amount {Amount}", orderId, amount);
        
        await Task.Delay(100, cancellationToken);
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"ch_{Guid.NewGuid().ToString()[..8]}"
        };
    }

    public async Task<PaymentResult> RefundPaymentAsync(string paymentId, decimal amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Processing Stripe refund for payment {PaymentId}, amount {Amount}", paymentId, amount);
        
        await Task.Delay(100, cancellationToken);
        
        return new PaymentResult
        {
            Success = true,
            TransactionId = $"re_{Guid.NewGuid().ToString()[..8]}"
        };
    }
}
