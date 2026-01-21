namespace WebApp.PaymentGateways;

public class PaymentGatewayFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;

    public PaymentGatewayFactory(IServiceProvider serviceProvider, IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
    }

    public IPaymentGateway GetPaymentGateway(string orderId)
    {
        var gatewayType = _configuration["Payment:DefaultGateway"] ?? "Stripe";
        
        return gatewayType.ToLower() switch
        {
            "stripe" => _serviceProvider.GetRequiredService<StripePaymentGateway>(),
            "paypal" => _serviceProvider.GetRequiredService<PayPalPaymentGateway>(),
            _ => _serviceProvider.GetRequiredService<StripePaymentGateway>()
        };
    }
}
