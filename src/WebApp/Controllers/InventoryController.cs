using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApp.Data;
using WebApp.Models;

namespace WebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(AppDbContext context, ILogger<InventoryController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllInventory()
    {
        var items = await _context.Inventory
            .Find(_ => true)
            .SortBy(i => i.ProductId)
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{productId}")]
    public async Task<IActionResult> GetInventoryByProductId(string productId)
    {
        var filter = Builders<InventoryItem>.Filter.Eq(i => i.ProductId, productId);
        var item = await _context.Inventory.Find(filter).FirstOrDefaultAsync();

        if (item == null)
        {
            return NotFound(new { message = $"Product {productId} not found in inventory" });
        }

        return Ok(item);
    }
}
