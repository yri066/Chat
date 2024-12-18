using Chat.Domain.Entities;


namespace Chat.Domain.Interface
{
    public interface IMessageSend
    {
        public Task SendMessage(Message message);

        public Task SendMessageTo(string userId, string textMessage);

        public void SendMessageDelay(Message message, int delay);

        public void SendMessageToDelay(string userId, string textMessage, int delay);
    }
}
