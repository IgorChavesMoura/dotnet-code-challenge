namespace WebApp.Dto;

public class PlaceOrderDto
{
    public string OrderId { get; set; } = string.Empty;
    public DateTime Time { get; set; }
    public List<ProductDto> Products { get; set; } = new();
    public decimal Value { get; set; }
}

public class ProductDto
{
    public string ProductId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Value { get; set; }
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
