using Chat.Domain.Entities;
using Chat.Domain.Interface.ConsoleWorker;
using Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleWorker.Workers
{
    /// <summary>
    /// Получить отправленные сообщения.
    /// </summary>
    public class GetSentMessages : IWorker
    {
        public string Name => "Получить список отправленных сообщений.";
        private readonly ApplicationDbContext _context;
        private readonly ChatConfig _chatConfig;

        public GetSentMessages(ApplicationDbContext context, IOptions<ChatConfig> options)
        {
            _context = context;
            _chatConfig = options.Value;
        }

        public async Task Run()
        {
            var currentId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.SenderId == currentId)
                .AsNoTracking()
                .ToListAsync();

            if (list.Count == 0)
            {
                Console.WriteLine("Нет никаких сообщений");
                return;
            }
            foreach (var message in list)
            {
                Console.WriteLine($"{message.RecipientId}-> {message.Text}");
            }
        }
    }
}
