namespace Chat.Interface
{
    public interface IMessageObserver
    {
        Task Notify(Message message);
    }
}
