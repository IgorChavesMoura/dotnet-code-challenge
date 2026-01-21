using WebApp.Data;
using WebApp.Messaging;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AppDbContext>();

builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddSingleton<KafkaOrderProducer>();

builder.Services.AddScoped<OrderReportGenerator>();
builder.Services.AddSingleton<OrderProcessingMetrics>();
builder.Services.AddSingleton<WebApp.Events.OrderEventPublisher>();
builder.Services.AddScoped<WebApp.Events.OrderNotificationService>();

builder.Services.AddScoped<WebApp.PaymentGateways.StripePaymentGateway>();
builder.Services.AddScoped<WebApp.PaymentGateways.PayPalPaymentGateway>();
builder.Services.AddScoped<WebApp.PaymentGateways.PaymentGatewayFactory>();

builder.Services.AddHostedService<KafkaOrderConsumer>();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5100);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
