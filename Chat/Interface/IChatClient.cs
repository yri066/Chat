﻿namespace Chat.Interface
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string sender, string message);
    }
}
