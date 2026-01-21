using System.Collections.Concurrent;

namespace WebApp.Services;

public class OrderProcessingMetrics
{
    private int _processedCount = 0;
    private int _failedCount = 0;
    
    public void IncrementProcessed()
    {
        _processedCount++;
    }
    
    public void IncrementFailed()
    {
        _failedCount++;
    }
    
    public (int processed, int failed) GetMetrics()
    {
        return (_processedCount, _failedCount);
    }
}
