﻿using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Chat.Infrastructure.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.SignalR
{
    /// <summary>
    /// Отправка сообщений в SignalR.
    /// </summary>
    public class SignalRMessageObserver : IMessageObserver
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly ChatConfig _config;

        public SignalRMessageObserver(IHubContext<ChatHub> hubContext, IOptions<ChatConfig> options)
        {
            _hubContext = hubContext;
            _config = options.Value;
        }

        /// <summary>
        /// Отправка сообщений в SignalR.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public async Task Notify(Message message)
        {
            var userId = _config.ClientId.ToString();

            if (message.RecipientId == userId)
            {
                await SendMessage(message.SenderId, message.SenderId, message.Text);
            }
            else if (message.SenderId == userId ||
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
