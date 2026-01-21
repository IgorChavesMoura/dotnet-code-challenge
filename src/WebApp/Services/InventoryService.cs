using MongoDB.Driver;
using System.Text.Json;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Services;

public class InventoryService
{
    private readonly AppDbContext _context;
    private readonly ILogger<InventoryService> _logger;

    public InventoryService(AppDbContext context, ILogger<InventoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> CheckAndReserveInventoryAsync(List<OrderItem> items)
    {
        foreach (var item in items)
        {
            var filter = Builders<InventoryItem>.Filter.Eq(i => i.ProductId, item.ProductId);
            var inventoryItem = await _context.Inventory.Find(filter).FirstOrDefaultAsync();

            if (inventoryItem == null)
            {
                _logger.LogWarning("Product {ProductId} not found in inventory", item.ProductId);
                return false;
            }

            if (inventoryItem.AvailableQuantity < item.Amount)
            {
                _logger.LogWarning("Insufficient inventory for {ProductId}. Available: {Available}, Requested: {Requested}",
                    item.ProductId, inventoryItem.AvailableQuantity, item.Amount);
                return false;
            }

            var update = Builders<InventoryItem>.Update
                .Inc(i => i.AvailableQuantity, -item.Amount)
                .Inc(i => i.ReservedQuantity, item.Amount)
                .Set(i => i.LastUpdated, DateTime.UtcNow);

            await _context.Inventory.UpdateOneAsync(filter, update);
        }

        return true;
    }

    public async Task PublishInventoryUpdateAsync(string orderId, List<OrderItem> items)
    {
        var inventoryUpdate = new
        {
            orderId,
            items = items.Select(i => new { i.ProductId, i.Amount }),
            timestamp = DateTime.UtcNow
        };

        _logger.LogInformation("Publishing inventory update for order {OrderId}", orderId);
    }
}
