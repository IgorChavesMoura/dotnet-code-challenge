using MongoDB.Driver;
using WebApp.Data;
using WebApp.Dto;
using WebApp.Models;

namespace WebApp.Services;

public class OrderService
{
    private readonly AppDbContext _context;
    private readonly InventoryService _inventoryService;
    private readonly PaymentService _paymentService;
    private readonly ILogger<OrderService> _logger;
    private readonly OrderReportGenerator _reportGenerator;
    private readonly OrderProcessingMetrics _metrics;
    private readonly WebApp.Events.OrderEventPublisher _eventPublisher;

    public OrderService(
        AppDbContext context, 
        InventoryService inventoryService,
        PaymentService paymentService,
        ILogger<OrderService> logger,
        OrderReportGenerator reportGenerator,
        OrderProcessingMetrics metrics,
        WebApp.Events.OrderEventPublisher eventPublisher)
    {
        _context = context;
        _inventoryService = inventoryService;
        _paymentService = paymentService;
        _logger = logger;
        _reportGenerator = reportGenerator;
        _metrics = metrics;
        _eventPublisher = eventPublisher;
    }

    public async Task<Order> CreateOrderAsync(PlaceOrderDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.OrderId))
            throw new ArgumentException("OrderId is required");

        if (dto.Products == null || dto.Products.Count == 0)
            throw new ArgumentException("At least one product is required");

        var order = new Order
        {
            OrderId = dto.OrderId,
            CreatedAt = dto.Time,
            TotalValue = dto.Value,
            Items = dto.Products.Select(p => new OrderItem
            {
                ProductId = p.ProductId,
                Name = p.Name,
                Value = p.Value,
                Amount = p.Amount
            }).ToList()
        };

        await _context.Orders.InsertOneAsync(order);

        return order;
    }

    public async Task ProcessOrderFromKafkaAsync(string orderId, string messageJson)
    {
        try
        {
            var order = await GetOrderByIdAsync(orderId);
            if (order != null)
            {
                return;
            }

            var dto = System.Text.Json.JsonSerializer.Deserialize<PlaceOrderDto>(messageJson);
            if (dto != null)
            {
                _logger.LogInformation("Processing order {OrderId} from Kafka", orderId);
                
                var inventoryAvailable = await _inventoryService.CheckAndReserveInventoryAsync(
                    dto.Products.Select(p => new OrderItem
                    {
                        ProductId = p.ProductId,
                        Name = p.Name,
                        Value = p.Value,
                        Amount = p.Amount
                    }).ToList());

                if (!inventoryAvailable)
                {
                    _logger.LogWarning("Insufficient inventory for order {OrderId}", orderId);
                    _metrics.IncrementFailed();
                    _eventPublisher.PublishStatusChange(orderId, "InsufficientInventory");
                    return;
                }

                var createdOrder = await CreateOrderAsync(dto);
                _eventPublisher.PublishStatusChange(orderId, "Created");
                
                await _inventoryService.PublishInventoryUpdateAsync(orderId, createdOrder.Items);

                var payment = await _paymentService.ProcessPaymentAsync(orderId, dto.Value);
                _eventPublisher.PublishStatusChange(orderId, "PaymentProcessed");
                
                await _paymentService.PublishPaymentEventAsync(payment);
                
                _metrics.IncrementProcessed();
                _logger.LogInformation("Order {OrderId} fully processed", orderId);
                
                var recentOrders = await GetRecentOrdersAsync(10);
                var report = _reportGenerator.GenerateOrderSummary(recentOrders);
                _logger.LogInformation("Generated report for {Count} orders. History size: {HistorySize}", 
                    recentOrders.Count, _reportGenerator.GetHistoryCount());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId}", orderId);
            _metrics.IncrementFailed();
            _eventPublisher.PublishStatusChange(orderId, "Failed");
            throw;
        }
    }

    public async Task<Order?> GetOrderByIdAsync(string orderId)
    {
        var filter = Builders<Order>.Filter.Eq(o => o.OrderId, orderId);
        return await _context.Orders.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetAllOrdersAsync()
    {
        return await _context.Orders
            .Find(Builders<Order>.Filter.Empty)
            .SortByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    private async Task<List<Order>> GetRecentOrdersAsync(int count)
    {
        return await _context.Orders
            .Find(Builders<Order>.Filter.Empty)
            .SortByDescending(o => o.CreatedAt)
            .Limit(count)
            .ToListAsync();
    }
}
