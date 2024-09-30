using Chat.Interface;
using Hangfire;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Chat.Services.Hubs
{

    public class ChatHub : Hub<IChatClient>
    {
        private readonly ChatConfig _chatConfig;
        private readonly IProducer _producer;
        public ChatHub(IOptions<ChatConfig> options, IProducer producer)
        {
            _chatConfig = options.Value;
            _producer = producer;
        }

        public async Task JoinChat(string userId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

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

        public void SendMessageDelay(string userId, string textMessage, string delay)
        {
            if (!int.TryParse(delay, out int delayResult))
                return;

            string job = BackgroundJob.Schedule(() =>
                SendMessage(userId, textMessage), TimeSpan.FromSeconds(delayResult));
        }
    }
}
