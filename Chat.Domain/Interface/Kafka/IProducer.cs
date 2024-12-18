using Chat.Domain.Entities;

namespace Chat.Domain
{
    public interface IProducer
    {
        Task SendMessage(Message message);
    }
}
