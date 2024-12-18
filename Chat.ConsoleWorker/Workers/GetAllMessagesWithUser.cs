using Chat.Domain.Entities;
using Chat.Domain.Interface.ConsoleWorker;
using Chat.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleWorker.Workers
{
    /// <summary>
    /// Получить все сообщения с пользователем.
    /// </summary>
    public class GetAllMessagesWithUser : IWorker
    {
        public string Name => "Получить список всех сообщений c пользователем.";
        private readonly ApplicationDbContext _context;
        private readonly ChatConfig _chatConfig;

        public GetAllMessagesWithUser(ApplicationDbContext context, IOptions<ChatConfig> options)
        {
            _context = context;
            _chatConfig = options.Value;
        }

        public async Task Run()
        {
            var userId = string.Empty;

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Введите id пользователя или 'quit' чтобы выйти:");
                userId = Console.ReadLine();

                if (string.IsNullOrEmpty(userId))
                {
                    continue;
                }

                if ("quit" == userId)
                {
                    return;
                }

                if (!_chatConfig.ChatList.Contains(userId))
                {
                    Console.WriteLine($"Пользователь {userId} не найден.");
                    continue;
                }

                break;
            }

            var currentId = _chatConfig.ClientId.ToString();
            var list = await _context.Messages.Where(x => x.RecipientId == userId ||
                                                    x.RecipientId == currentId && x.SenderId == userId)
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
