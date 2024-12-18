namespace Chat.Domain.Interface.SignalR
{
    public interface IChatClient
    {
        public Task ReceiveMessage(string sender, string message);
    }
}
