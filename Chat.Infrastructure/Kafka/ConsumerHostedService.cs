using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Chat.Infrastructure.Kafka
{
    /// <summary>
    /// Kafka Consumer.
    /// </summary>
    public class ConsumerHostedService : IHostedService
    {
        private readonly KafkaConfig _kafkaConfig;
        private readonly ChatConfig _chatConfig;
        private readonly ILogger _logger;
        private readonly IMessageSubject _messageSubject;

        private CancellationTokenSource _cst;
        private Task _runConsumer;

        public ConsumerHostedService(IOptions<KafkaConfig> kafka, IOptions<ChatConfig> chat, IMessageSubject messageSubject, ILogger<ConsumerHostedService> logger)
        {
            _kafkaConfig = kafka.Value;
            _chatConfig = chat.Value;
            _messageSubject = messageSubject;
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

        /// <summary>
        /// Получение сообщений из Kafka.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        public async Task RunConsumer(CancellationToken cancellationToken)
        {
            var config = new ConsumerConfig
            {
                GroupId = _kafkaConfig.ConsumerGroupId,
                BootstrapServers = _kafkaConfig.BootstrapServers,
                AutoOffsetReset = AutoOffsetReset.Earliest,
            };

            using (var consumer = new ConsumerBuilder<string, string>(config).Build())
            {
                consumer.Subscribe(_kafkaConfig.Topic);

                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(cancellationToken);

                        if (consumeResult is null)
                        {
                            continue;
                        }

                        var messageJson = consumeResult.Message.Value;
                        var message = JsonSerializer.Deserialize<Message>(messageJson);

                        if (message != null)
                        {
                            await CheckMessageAccess(message);
                        }
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

        /// <summary>
        /// Уведомляет наблюдателей о доступном сообщении.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        private async Task CheckMessageAccess(Message message)
        {
            var userId = _chatConfig.ClientId.ToString();
            var publicChatId = _chatConfig.PublicChatId;

            // Проверка сообщения, относится ли оно к пользователю.
            if (message.SenderId == userId ||
                message.RecipientId == userId ||
                message.RecipientId == publicChatId)
            {
                await _messageSubject.NotifyObservers(message);
            }
        }
    }
}
