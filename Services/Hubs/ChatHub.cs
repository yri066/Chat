using Chat.Interface;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Chat.Services.Hubs
{
    /// <summary>
    /// Хаб SignalR.
    /// </summary>
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ChatConfig _chatConfig;
        private readonly IProducer _producer;
        public ChatHub(IOptions<ChatConfig> options, IProducer producer)
        {
            _chatConfig = options.Value;
            _producer = producer;
        }

        /// <summary>
        /// Присоединиться к группе.
        /// </summary>
        /// <param name="userId">Id пользователя.</param>
        /// <returns></returns>
        public async Task JoinChat(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        /// <summary>
        /// Отправить сообщение.
        /// </summary>
        /// <param name="userId">Id получателя.</param>
        /// <param name="textMessage">Текст сообщения.</param>
        /// <returns></returns>
        public async Task SendMessage(string userId, string textMessage)
        {
            var message = new Message
            {
                SenderId = _chatConfig.ClientId.ToString(),
                RecipientId = userId,
                Text = textMessage
            };

            await _producer.SendMessage(message);
        }

        /// <summary>
        /// Отправить сообщение с задержкой.
        /// </summary>
        /// <param name="userId">Id получателя.</param>
        /// <param name="textMessage">Текст сообщения.</param>
        /// <param name="delay">Задержка в секундах перед отправкой сообщения.</param>
        public void SendMessageDelay(string userId, string textMessage, string delay)
        {
            if (!int.TryParse(delay, out int delayResult))
                return;

            string job = BackgroundJob.Schedule(() =>
                SendMessage(userId, textMessage), TimeSpan.FromSeconds(delayResult));
        }
    }
}
