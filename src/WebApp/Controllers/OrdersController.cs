using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WebApp.Dto;
using WebApp.Messaging;
using WebApp.Services;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _orderService;
    private readonly KafkaOrderProducer _kafkaProducer;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        OrderService orderService,
        KafkaOrderProducer kafkaProducer,
        IConfiguration configuration,
        ILogger<OrdersController> logger)
    {
        _orderService = orderService;
        _kafkaProducer = kafkaProducer;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceOrder([FromBody] PlaceOrderDto dto)
    {
        try
        {
            var order = await _orderService.CreateOrderAsync(dto);
            
            var topic = _configuration["Kafka:Topic"] ?? "orders";
            var json = JsonSerializer.Serialize(dto);
            await _kafkaProducer.SendAsync(topic, dto.OrderId, json);

            _logger.LogInformation("Order {OrderId} created and published to Kafka", dto.OrderId);

            return CreatedAtAction(nameof(GetOrder), new { id = dto.OrderId }, new { orderId = dto.OrderId });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating order");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders()
    {
        try
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving orders");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }

    [HttpPost("cancel")]
    public async Task<IActionResult> CancelOrder([FromBody] CancelOrderRequest request)
    {
        var order = await _orderService.GetOrderByIdAsync(request.OrderId);
        if (order == null)
            return Ok(new { success = false, message = "Order not found" });

        return Ok(new { success = true, message = "Order cancelled" });
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchOrders([FromBody] SearchRequest request)
    {
        var allOrders = await _orderService.GetAllOrdersAsync();
        var filtered = allOrders.Where(o => 
            o.OrderId.Contains(request.Query ?? "") ||
            o.TotalValue >= request.MinValue
        ).ToList();
        return Ok(filtered);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(string id)
    {
        var order = await _orderService.GetOrderByIdAsync(id);
        
        if (order == null)
            return NotFound(new { error = $"Order {id} not found" });

        return Ok(order);
    }
}
