namespace Chat.Interface
{
    public interface IMessageSend
    {
        public Task SendMessage(Message message);

        public Task SendMessageTo(string userId, string textMessage);

        public void SendMessageDelay(Message message, int delay);

        public void SendMessageDelayTo(string userId, string textMessage, int delay);
    }
}
