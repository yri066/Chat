using Chat.Domain;
using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Hangfire;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.Kafka
{
    /// <summary>
    /// Отправка сообщения через Kafka.
    /// </summary>
    public class MessageSend : IMessageSend
    {
        private readonly IProducer _producer;
        private readonly ChatConfig _config;
        public MessageSend(IProducer producer, IOptions<ChatConfig> options)
        {
            _producer = producer;
            _config = options.Value;
        }

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public async Task SendMessage(Message message)
        {
            await _producer.SendMessage(message);
        }

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <param name="textMessage">Текст сообщения.</param>
        public async Task SendMessageTo(string userId, string textMessage)
        {
            var message = new Message
            {
                SenderId = _config.ClientId.ToString(),
                RecipientId = userId,
                Text = textMessage
            };

            await SendMessage(message);
        }

        /// <summary>
        /// Отправить сообщение с задержкой.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        /// <param name="delay">Задержка в секундах перед отправкой сообщения.</param>
        public void SendMessageDelay(Message message, int delay)
        {
            string job = BackgroundJob.Schedule(() =>
                SendMessage(message), TimeSpan.FromSeconds(delay));
        }

        /// <summary>
        /// Отправить сообщение с задержкой.
        /// </summary>
        /// <param name="userId">Id получателя.</param>
        /// <param name="textMessage">Текст сообщения.</param>
        /// <param name="delay">Задержка в секундах перед отправкой сообщения.</param>
        public void SendMessageToDelay(string userId, string textMessage, int delay)
        {
            string job = BackgroundJob.Schedule(() =>
                SendMessageTo(userId, textMessage), TimeSpan.FromSeconds(delay));
        }
    }
}
