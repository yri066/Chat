﻿using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Chat.Domain.Interface.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace Chat.Infrastructure.SignalR.Hubs
{
    /// <summary>
    /// Хаб SignalR.
    /// </summary>
    public class ChatHub : Hub<IChatClient>
    {
        private readonly ChatConfig _chatConfig;
        private readonly IMessageSend _sendMessage;

        public ChatHub(IOptions<ChatConfig> options, IMessageSend sendMessage)
        {
            _chatConfig = options.Value;
            _sendMessage = sendMessage;
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

            await _sendMessage.SendMessage(message);
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

            _sendMessage.SendMessageToDelay(userId, textMessage, delayResult);
        }
    }
}
