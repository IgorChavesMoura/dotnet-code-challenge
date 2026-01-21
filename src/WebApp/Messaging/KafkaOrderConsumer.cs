using Confluent.Kafka;
using WebApp.Services;

namespace WebApp.Messaging;

public class KafkaOrderConsumer : BackgroundService
{
    private readonly ILogger<KafkaOrderConsumer> _logger;
    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private IConsumer<string, string>? _consumer;

    public KafkaOrderConsumer(ILogger<KafkaOrderConsumer> logger, IConfiguration configuration, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _configuration = configuration;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = $"order-consumer-{Guid.NewGuid()}",
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = true,
            AutoCommitIntervalMs = 5000
        };

        _consumer = new ConsumerBuilder<string, string>(config).Build();

        var topic = _configuration["Kafka:Topic"] ?? "orders";
        _consumer.Assign(new TopicPartition(topic, new Partition(0)));

        _logger.LogInformation("Kafka consumer started with group: {GroupId}", config.GroupId);

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromSeconds(1));
                    
                    if (consumeResult != null)
                    {
                        _logger.LogInformation(
                            "Consumed message from partition {Partition} at offset {Offset}: {Key}",
                            consumeResult.Partition.Value,
                            consumeResult.Offset.Value,
                            consumeResult.Message.Key);

                        try
                        {
                            using var scope = _serviceProvider.CreateScope();
                            var orderService = scope.ServiceProvider.GetRequiredService<OrderService>();
                            await orderService.ProcessOrderFromKafkaAsync(
                                consumeResult.Message.Key,
                                consumeResult.Message.Value);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing message {Key}", consumeResult.Message.Key);
                        }
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in consumer loop");
                }
            }
        }
        finally
        {
            _consumer.Close();
            _logger.LogInformation("Kafka consumer stopped");
        }
    }

    public override void Dispose()
    {
        _consumer?.Dispose();
        base.Dispose();
    }
}
