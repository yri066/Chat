using Confluent.Kafka;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Chat.Services.Kafka
{
    public class Producer : IProducer
    {
        private readonly KafkaConfig _config;
        private readonly ILogger _logger;
        public Producer(IOptions<KafkaConfig> options, ILogger<Producer> logger)
        {
            _config = options.Value;
            _logger = logger;
        }

        public async Task SendMessage(Message message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _config.BootstrapServers,
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                var messageJson = JsonSerializer.Serialize(message);

                try
                {
                    await producer.ProduceAsync(_config.Topic, new Message<Null, string> { Value = messageJson });
                }
                catch(ProduceException<Null, string> ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}
