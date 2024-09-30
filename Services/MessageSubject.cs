using Chat.Interface;

namespace Chat.Services
{
    public class MessageSubject : IMessageSubject
    {
        private readonly List<IMessageObserver> _observers = new();

        public void Attach(IMessageObserver observer)
        {
            _observers.Add(observer);
        }

        public void Detach(IMessageObserver observer)
        {
            _observers.Remove(observer);
        }

        public async Task NotifyObservers(Message message)
        {
            foreach (var observer in _observers)
            {
                await observer.Notify(message);
            }
        }
    }
}
