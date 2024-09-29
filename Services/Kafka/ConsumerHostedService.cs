
using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace Chat.Services.Kafka
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly KafkaConfig _config;
        private readonly ILogger _logger;

        private CancellationTokenSource _cst;
        private Task _runConsumer;

        public ConsumerHostedService(IOptions<KafkaConfig> options, ILogger<ConsumerHostedService> logger)
        {
            _config = options.Value;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka consumer starting.");
            _cst = new CancellationTokenSource();
            Task.Run(() => RunConsumer(_cst.Token), _cst.Token);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Kafka consumer stopping.");
            _cst.Cancel();
            _cst.Dispose();

            return Task.CompletedTask;
        }

        public void RunConsumer(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _config.ConsumerGroupId,
                BootstrapServers = _config.BootstrapServers,
                AllowAutoCreateTopics = true,
            };

            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Subscribe(_config.Topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumerResult = consumer.Consume(cancellationToken);
                        Console.WriteLine($"Consume message: {consumerResult.Message.Value}");
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (ConsumeException e)
                    {
                        _logger.LogInformation($"Consume error: {e.Error.Reason}");

                        if (e.Error.IsFatal)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                }
            }
        }
    }
}
