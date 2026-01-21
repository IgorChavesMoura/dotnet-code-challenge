using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApp.Models;

public class Payment
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("paymentId")]
    public string PaymentId { get; set; } = string.Empty;
    
    [BsonElement("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [BsonElement("amount")]
    public decimal Amount { get; set; }
    
    [BsonElement("status")]
    public string Status { get; set; } = "Pending";
    
    [BsonElement("processedAt")]
    public DateTime ProcessedAt { get; set; }
}
