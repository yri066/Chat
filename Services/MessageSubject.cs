using Chat.Interface;

namespace Chat.Services
{
    /// <summary>
    /// Уведомить о сообщении.
    /// </summary>
    public class MessageSubject : IMessageSubject
    {
        private readonly List<IMessageObserver> _observers = new();

        /// <summary>
        /// Добавить наблюдателя.
        /// </summary>
        /// <param name="observer">Наблюдатель.</param>
        public void Attach(IMessageObserver observer)
        {
            _observers.Add(observer);
        }

        /// <summary>
        /// Убрать наблюдателя.
        /// </summary>
        /// <param name="observer">Наблюдатель.</param>
        public void Detach(IMessageObserver observer)
        {
            _observers.Remove(observer);
        }

        /// <summary>
        /// Уведомление наблюдателей о сообщении.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public async Task NotifyObservers(Message message)
        {
            foreach (var observer in _observers)
            {
                await observer.Notify(message);
            }
        }
    }
}
