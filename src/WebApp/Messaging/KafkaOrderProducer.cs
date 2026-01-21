using Confluent.Kafka;

namespace WebApp.Messaging;

public class KafkaOrderProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaOrderProducer> _logger;

    public KafkaOrderProducer(IConfiguration configuration, ILogger<KafkaOrderProducer> logger)
    {
        _logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            Acks = Acks.All,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task SendAsync(string topic, string key, string json)
    {
        try
        {
            var result = await _producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = key,
                Value = json
            });

            _logger.LogInformation("Message sent to {Topic} partition {Partition} at offset {Offset}", 
                result.Topic, result.Partition.Value, result.Offset.Value);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to produce message to Kafka");
            throw;
        }
    }
}
