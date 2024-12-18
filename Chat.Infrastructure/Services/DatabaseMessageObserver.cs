using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Chat.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Chat.Infrastructure.Services
{
    /// <summary>
    /// Сохранение сообщения в бд.
    /// </summary>
    public class DatabaseMessageObserver : IMessageObserver
    {
        private readonly IServiceProvider _provider;
        public DatabaseMessageObserver(IServiceProvider provider)
        {
            _provider = provider;
        }

        /// <summary>
        /// Сохранение сообщения в бд.
        /// </summary>
        /// <param name="message">Сообщение.</param>
        public async Task Notify(Message message)
        {
            using (IServiceScope scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                context.Messages.Add(message);
                await context.SaveChangesAsync();
            }
        }
    }
}
