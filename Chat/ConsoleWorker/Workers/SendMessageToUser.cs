using Chat.ConsoleWorker.Interface;
using Chat.Interface;
using Microsoft.Extensions.Options;

namespace Chat.ConsoleWorker.Workers
{
    /// <summary>
    /// Отправить сообщение пользователю.
    /// </summary>
    public class SendMessageToUser: IWorker
    {
        public virtual string Name => "Отправить сообщение пользователю.";
        private readonly ApplicationDbContext _context;
        private readonly ChatConfig _chatConfig;
        private readonly IMessageSend _messageSend;
        private readonly CancellationTokenSource _cts;

        public SendMessageToUser(ApplicationDbContext context, IOptions<ChatConfig> options, IMessageSend messageSend)
        {
            _cts = new CancellationTokenSource();
            _context = context;
            _chatConfig = options.Value;
            _messageSend = messageSend;
        }

        public async Task Run()
        {
            var token = _cts.Token;

            var delaty = 0;
            bool isDelay = false;
            var userId = string.Empty;
            var messageText = string.Empty;

            isDelay = MessageWithDelay(_cts);

            if (isDelay)
            {
                delaty = GetDelay(_cts);
            }
            
            userId = GetUserId(_cts);


            while (!token.IsCancellationRequested)
            {
                Console.WriteLine();
                Console.WriteLine("Введите текст сообщения или 'quit' чтобы выйти:");

                messageText = Console.ReadLine();

                if (string.IsNullOrEmpty(messageText))
                {
                    continue;
                }

                if ("quit" == messageText)
                {
                    return;
                }

                if (!isDelay)
                {
                    await _messageSend.SendMessageTo(userId, messageText);
                }
                else
                {
                    _messageSend.SendMessageDelayTo(userId, messageText, delaty);
                }

                return;
            }
        }

        /// <summary>
        /// Отправить сообщение с задержкой или без.
        /// </summary>
        /// <param name="cts">Токен отмены.</param>
        /// <returns>С задержкой или без.</returns>
        protected bool MessageWithDelay(CancellationTokenSource cts)
        {
            if(cts.IsCancellationRequested)
            {
                return false;
            }

            while (true)
            {
                Console.WriteLine("Отправить сообщение с задержкой?: 0 - да, 1 - нет или 'quit' чтобы выйти:");

                var act = Console.ReadLine();

                if (string.IsNullOrEmpty(act))
                {
                    continue;
                }

                if ("quit" == act)
                {
                    cts.Cancel();
                    return false;
                }

                if (!byte.TryParse(act, out var val))
                {
                    Console.WriteLine("Не удалось распознать команду {0}", act);
                    continue;
                }

                if (val >= 2)
                {
                    Console.WriteLine("Значение {0} слишком большое.", val);
                    continue;
                }

                return val == 0 ? true : false;
            }
        }

        /// <summary>
        /// Получить длительность задержки.
        /// </summary>
        /// <param name="cts">Токен отмены.</param>
        /// <returns>Длительность задержки.</returns>
        protected int GetDelay(CancellationTokenSource cts)
        {
            if (cts.IsCancellationRequested)
            {
                return 0;
            }

            while (true)
            {
                Console.WriteLine("Введите длительность задержки или 'quit' чтобы выйти:");

                var act = Console.ReadLine();

                if (string.IsNullOrEmpty(act))
                {
                    continue;
                }

                if ("quit" == act)
                {
                    cts.Cancel();
                    return 0;
                }

                if (!int.TryParse(act, out var val))
                {
                    Console.WriteLine("Не удалось распознать длительность задержки {0}", act);
                    continue;
                }

                if (val < 0)
                {
                    Console.WriteLine("Значение {0} не может быть отрицательным.", val);
                    continue;
                }

                return val;
            }
        }

        /// <summary>
        /// Получить Id пользователя.
        /// </summary>
        /// <param name="cts">Токен отмены.</param>
        /// <returns>Id пользователя.</returns>
        protected virtual string GetUserId(CancellationTokenSource cts)
        {
            if (cts.IsCancellationRequested)
            {
                return string.Empty;
            }

            var userId = string.Empty;
            
            while (true)
            {
                Console.WriteLine("Введите id пользователя или 'quit' чтобы выйти:");
                userId = Console.ReadLine();

                if (string.IsNullOrEmpty(userId))
                {
                    continue;
                }

                if ("quit" == userId)
                {
                    cts.Cancel();
                    return string.Empty;
                }

                if (!_chatConfig.ChatList.Contains(userId))
                {
                    Console.WriteLine($"Пользователь {userId} не найден.");
                    continue;
                }

                return userId;
            }
        }
    }
}
