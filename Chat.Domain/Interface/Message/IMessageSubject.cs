using Chat.Domain.Entities;

namespace Chat.Domain.Interface
{
    public interface IMessageSubject
    {
        void Attach(IMessageObserver observer);
        void Detach(IMessageObserver observer);
        Task NotifyObservers(Message message);
    }
}
