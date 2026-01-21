using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Models;

public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [BsonElement("totalValue")]
    public decimal TotalValue { get; set; }
    
    [BsonElement("items")]
    public List<OrderItem> Items { get; set; } = new();
}
