using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Chat.Domain.Interface.ConsoleWorker;
using Chat.Infrastructure.Data;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleWorker.Workers
{
    /// <summary>
    /// Отправить сообщение всем.
    /// </summary>
    public class SendMessageToAll : SendMessageToUser, IWorker
    {
        public override string Name => "Отправить сообщение всем пользователям.";
        private readonly ApplicationDbContext _context;
        private readonly ChatConfig _chatConfig;
        private readonly IMessageSend _messageSend;

        public SendMessageToAll(ApplicationDbContext context, IOptions<ChatConfig> options, IMessageSend messageSend) :
            base(context, options, messageSend)
        {
            _context = context;
            _chatConfig = options.Value;
            _messageSend = messageSend;
        }

        protected override string GetUserId(CancellationTokenSource cts)
        {
            return _chatConfig.PublicChatId;
        }
    }
}
