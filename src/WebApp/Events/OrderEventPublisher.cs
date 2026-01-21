namespace WebApp.Events;

public class OrderStatusChangedEvent
{
    public string OrderId { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
}

public class OrderEventPublisher
{
    public event EventHandler<OrderStatusChangedEvent>? OrderStatusChanged;
    
    public void PublishStatusChange(string orderId, string newStatus)
    {
        OrderStatusChanged?.Invoke(this, new OrderStatusChangedEvent
        {
            OrderId = orderId,
            NewStatus = newStatus,
            ChangedAt = DateTime.UtcNow
        });
    }
}

public class OrderNotificationService
{
    private readonly OrderEventPublisher _eventPublisher;
    private readonly ILogger<OrderNotificationService> _logger;

    public OrderNotificationService(OrderEventPublisher eventPublisher, ILogger<OrderNotificationService> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
        
        _eventPublisher.OrderStatusChanged += OnOrderStatusChanged;
    }

    private void OnOrderStatusChanged(object? sender, OrderStatusChangedEvent e)
    {
        _logger.LogInformation("Order {OrderId} status changed to {Status}", e.OrderId, e.NewStatus);
    }
}
