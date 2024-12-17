using Chat.ConsoleWorker.Interface;
using Microsoft.EntityFrameworkCore;

namespace Chat.ConsoleWorker.Workers
{
    /// <summary>
    /// Получить все сообщения.
    /// </summary>
    public class GetAllMessages : IWorker
    {
        public string Name => "Получить список всех сообщений.";
        private readonly ApplicationDbContext _context;

        public GetAllMessages(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task Run()
        {
            var list = await _context.Messages.AsNoTracking().ToListAsync();

            if(list.Count == 0)
            {
                Console.WriteLine("Нет никаких сообщений");
                return;
            }
            foreach (var message in list)
            {
                Console.WriteLine($"{message.SenderId}->{message.RecipientId}: {message.Text}");
            }
        }
    }
}
