﻿using Chat.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Text.Json;

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
            await Clients.Group(userId).ReceiveMessage(userId, textMessage);
        }

        public async Task PostMessage(string userId, string sender, string textMessage)
        {
            await Clients.Group(userId).ReceiveMessage(sender, textMessage);
        }
    }
}
