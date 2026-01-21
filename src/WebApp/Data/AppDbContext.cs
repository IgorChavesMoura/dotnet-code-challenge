using MongoDB.Driver;
using WebApp.Models;

namespace WebApp.Data;

public class AppDbContext
{
    private readonly IMongoDatabase _database;

    public AppDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrdersDb");
        var mongoUrl = MongoUrl.Create(connectionString);
        var client = new MongoClient(connectionString);
        _database = client.GetDatabase(mongoUrl.DatabaseName ?? "orders");
    }

    public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");
    public IMongoCollection<InventoryItem> Inventory => _database.GetCollection<InventoryItem>("inventory");
    public IMongoCollection<Payment> Payments => _database.GetCollection<Payment>("payments");
}
