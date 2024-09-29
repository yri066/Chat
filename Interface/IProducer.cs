namespace Chat
{
    public interface IProducer
    {
        Task SendMessage(string message);
    }
}
