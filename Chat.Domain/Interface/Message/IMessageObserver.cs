using Chat.Domain.Entities;

namespace Chat.Domain.Interface
{
    public interface IMessageObserver
    {
        Task Notify(Message message);
    }
}
