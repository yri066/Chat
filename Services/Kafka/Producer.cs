using Confluent.Kafka;
using Microsoft.Extensions.Options;

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

        public async Task SendMessage(string message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _config.BootstrapServers,
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    await producer.ProduceAsync(_config.Topic, new Message<Null, string> { Value = message });
                }
                catch(ProduceException<Null, string> ex)
                {
                    _logger.LogError(ex.ToString());
                }
            }
        }
    }
}
