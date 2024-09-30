using Chat.Interface;
using Chat.Services.Hubs;
using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace Chat.Services
{
    public class SignalRMessageObserver : IMessageObserver
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatConfig _config;

        public SignalRMessageObserver(IHubContext<ChatHub> hubContext, IOptions<ChatConfig> options)
        {
            _hubContext = hubContext;
            _config = options.Value;
        }

        public async Task Notify(Message message)
        {
            var userId = _config.ClientId.ToString();

            if (message.RecipientId == userId)
            {
                await SendMessage(message.SenderId, message.RecipientId, message.Text);
            }
            else if(message.SenderId == userId ||
                    message.RecipientId == _config.PublicChatId)
            {
                await SendMessage(message.RecipientId, message.SenderId, message.Text);
            }
        }

        private async Task SendMessage(string groupId, string senderId, string text)
        {
            await _hubContext.Clients.Groups(groupId).SendAsync("ReceiveMessage", senderId, text);
        }
    }
}
