using System.Text.Json.Serialization;

namespace WebApp.Dto;

public class PlaceOrderDto
{
    [JsonPropertyName("orderId")]
    public string OrderId { get; set; } = string.Empty;
    
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }
    
    [JsonPropertyName("products")]
    public List<ProductDto> Products { get; set; } = new();
    
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}

public class ProductDto
{
    [JsonPropertyName("productId")]
    public string ProductId { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("value")]
    public decimal Value { get; set; }
    
    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}

public class CancelOrderRequest
{
    public string OrderId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}

public class SearchRequest
{
    public string? Query { get; set; }
    public decimal MinValue { get; set; }
}
