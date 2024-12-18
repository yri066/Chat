using Chat.ConsoleWorker.Service;
using Chat.ConsoleWorker.Workers;
using Chat.Domain.Interface.ConsoleWorker;
using Microsoft.Extensions.DependencyInjection;


namespace Chat.ConsoleWorker.Config
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Регистрирует обработчиков консольных действий.
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddConsoleWorker(this IServiceCollection services)
        {
            services.AddTransient<IWorker, GetAllMessages>();
            services.AddTransient<IWorker, GetAllMessagesWithUser>();
            services.AddTransient<IWorker, GetIncomingMessages>();
            services.AddTransient<IWorker, GetSentMessages>();
            services.AddTransient<IWorker, SendMessageToAll>();
            services.AddTransient<IWorker, SendMessageToUser>();

            services.AddHostedService<ConsoleService>();

            return services;
        }
    }
}
