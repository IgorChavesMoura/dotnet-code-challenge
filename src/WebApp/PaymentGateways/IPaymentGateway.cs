namespace WebApp.PaymentGateways;

public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(string orderId, decimal amount, CancellationToken cancellationToken = default);
    Task<PaymentResult> RefundPaymentAsync(string paymentId, decimal amount, CancellationToken cancellationToken = default);
}

public class PaymentResult
{
    public bool Success { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}
