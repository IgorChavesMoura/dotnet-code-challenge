using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Models;

public class InventoryItem
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("productId")]
    public string ProductId { get; set; } = string.Empty;
    
    [BsonElement("name")]
    public string Name { get; set; } = string.Empty;
    
    [BsonElement("availableQuantity")]
    public int AvailableQuantity { get; set; }
    
    [BsonElement("reservedQuantity")]
    public int ReservedQuantity { get; set; }
    
    [BsonElement("lastUpdated")]
    public DateTime LastUpdated { get; set; }
}
