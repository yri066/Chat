using Chat.Domain.Entities;
using Chat.Domain.Interface.ConsoleWorker;
using Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleWorker.Workers
{
    /// <summary>
    /// Получить входящие сообщения.
    /// </summary>
    public class GetIncomingMessages : IWorker
    {
        public string Name => "Получить список входящих сообщений.";
        private readonly ApplicationDbContext _context;
        private readonly ChatConfig _chatConfig;

        public GetIncomingMessages(ApplicationDbContext context, IOptions<ChatConfig> options)
        {
            _context = context;
            _chatConfig = options.Value;
        }

        public async Task Run()
        {
            var currentId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.RecipientId == currentId)
                .AsNoTracking()
                .ToListAsync();

            if (list.Count == 0)
            {
                Console.WriteLine("Нет никаких сообщений");
                return;
            }
            foreach (var message in list)
            {
                Console.WriteLine($"{message.SenderId}: {message.Text}");
            }
        }
    }
}
