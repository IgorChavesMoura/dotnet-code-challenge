using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Models;

public class OrderItem
{
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;
    
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("value")]
    public decimal Value { get; set; }
    
    [BsonElement("amount")]
    public int Amount { get; set; }
}
