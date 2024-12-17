namespace Chat
{
    public interface IProducer
    {
        Task SendMessage(Message message);
    }
}
