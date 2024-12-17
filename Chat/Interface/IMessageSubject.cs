using Chat.Interface;

namespace Chat
{
    public interface IMessageSubject
    {
        void Attach(IMessageObserver observer);
        void Detach(IMessageObserver observer);
        Task NotifyObservers(Message message);
    }
}
