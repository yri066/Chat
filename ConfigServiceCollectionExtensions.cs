using Chat.ConsoleWorker.Interface;
using Chat.ConsoleWorker.Workers;
using Chat.Interface;
using Chat.Services;
using Chat.Services.Kafka;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Chat
{
    public static class ConfigServiceCollectionExtensions
    {
        /// <summary>
        /// Настройка конфигурации.
        /// </summary>
        /// <param name="services">Коллекция служб.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddConfigs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<KafkaConfig>(configuration.GetSection(KafkaConfig.Position));
            services.Configure<ChatConfig>(configuration.GetSection(ChatConfig.Position));

            return services;
        }

        /// <summary>
        /// Регистрирует сервисы Kafka.
        /// </summary>
        /// <param name="services">Коллекция служб.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddKafkaServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IProducer, Producer>();
            services.AddHostedService<ConsumerHostedService>();
            services.AddSingleton<IMessageSend, MessageSend>();

            return services;
        }

        /// <summary>
        /// Регистрирует контекст базы данных PostgreSQL.
        /// </summary>
        /// <param name="services">Коллекция служб.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddDatabaseNpgsql(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            return services;
        }

        /// <summary>
        /// Регистрирует сервисы Hangfire.
        /// </summary>
        /// <param name="services">Коллекция служб.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddHangfireServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHangfire(h =>
            {
                h.UsePostgreSqlStorage(conf =>
                conf.UseNpgsqlConnection(configuration.GetConnectionString("Hangfire")));
            });
            services.AddHangfireServer(options =>
            {
                options.WorkerCount = 5;
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });
            services.AddHangfireServer();

            return services;
        }

        /// <summary>
        /// Регистрирует издателя и подписчиков на новые сообщений.
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddMessageObservers(this IServiceCollection services)
        {
            services.AddSingleton<SignalRMessageObserver>();
            services.AddSingleton<DatabaseMessageObserver>();

            services.AddSingleton<IMessageSubject, MessageSubject>(serviceProvider =>
            {
                var messageSubject = new MessageSubject();

                var signalRObserver = serviceProvider.GetRequiredService<SignalRMessageObserver>();
                var databaseObserver = serviceProvider.GetRequiredService<DatabaseMessageObserver>();

                messageSubject.Attach(signalRObserver);
                messageSubject.Attach(databaseObserver);

                return messageSubject;
            });

            return services;
        }

        /// <summary>
        /// Регистрирует обработчиков консольных действий.
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddConsoleWorkers(this IServiceCollection services)
        {
            services.AddTransient<IWorker, GetAllMessages>();
            services.AddTransient<IWorker, GetAllMessagesWithUser>();
            services.AddTransient<IWorker, GetIncomingMessages>();
            services.AddTransient<IWorker, GetSentMessages>();
            services.AddTransient<IWorker, SendMessageToAll>();
            services.AddTransient<IWorker, SendMessageToUser>();

            services.AddHostedService<ConsoleWorker.ConsoleWorker>();

            return services;
        }

        /// <summary>
        /// Регистрирует Swagger.
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen();

            return services;
        }

        /// <summary>
        /// Регистрирует различные сервисы.
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddDifferentServices(this IServiceCollection services)
        {
            services.AddSignalR();
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddRazorPages();

            return services;
        }
    }
}
