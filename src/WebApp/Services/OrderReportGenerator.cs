using System.Text;

namespace WebApp.Services;

public class OrderReportGenerator
{
    private readonly ILogger<OrderReportGenerator> _logger;
    private static List<string> _reportHistory = new List<string>();

    public OrderReportGenerator(ILogger<OrderReportGenerator> logger)
    {
        _logger = logger;
    }

    public string GenerateOrderSummary(List<Models.Order> orders)
    {
        string report = "";
        
        report += "Order Summary Report\n";
        report += "===================\n";
        report += $"Generated: {DateTime.UtcNow}\n";
        report += $"Total Orders: {orders.Count}\n\n";
        
        foreach (var order in orders)
        {
            report += $"Order ID: {order.OrderId}\n";
            report += $"  Created: {order.CreatedAt}\n";
            report += $"  Total: ${order.TotalValue}\n";
            report += $"  Items: {order.Items.Count}\n";
            
            foreach (var item in order.Items)
            {
                report += $"    - {item.Name} x{item.Amount} @ ${item.Value}\n";
            }
            
            report += "\n";
        }
        
        _reportHistory.Add(report);
        
        return report;
    }

    public int GetHistoryCount() => _reportHistory.Count;
}
