using Chat.Domain.Interface.ConsoleWorker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Chat.ConsoleWorker.Service
{
    /// <summary>
    /// Запуск доступных действий из консоли.
    /// </summary>
    public class ConsoleService : IHostedService
    {
        private readonly IServiceProvider _service;

        private CancellationTokenSource _cst;
        private Task _runWork;

        public ConsoleService(IServiceProvider service)
        {
            _service = service;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _cst = new CancellationTokenSource();
            _runWork = Task.Run(() => DoWork(_cst.Token), _cst.Token);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _cst.Cancel();
            _cst.Dispose();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Запускает на выполнение доступные действия.
        /// </summary>
        /// <param name="token">Токен отмены.</param>
        private async Task DoWork(CancellationToken token)
        {
            using (IServiceScope scope = _service.CreateScope())
            {
                var service = scope.ServiceProvider.GetServices(typeof(IWorker)).Cast<IWorker>().ToList();

                while (!token.IsCancellationRequested)
                {
                    Console.WriteLine();
                    Console.WriteLine("Доступные действия...");

                    for (var i = 0; i < service.Count; i++)
                    {
                        Console.WriteLine("{0}: {1}", i, service[i].Name);
                    }

                    while (!token.IsCancellationRequested)
                    {
                        Console.WriteLine("Введите номер действия (0..{0})", service.Count-1);
                        var act = Console.ReadLine();

                        if (string.IsNullOrEmpty(act))
                        {
                            await Task.Delay(2000);
                            continue;
                        }

                        if (!byte.TryParse(act, out var val))
                        {
                            Console.WriteLine("Не удалось распознать команду {0}", act);
                            continue;
                        }

                        if (val >= service.Count)
                        {
                            Console.WriteLine("Значение {0} слишком большое.", val);
                            continue;
                        }

                        var worker = service[val];

                        await worker.Run();

                        break;
                    }
                }
            }
        }
    }
}
