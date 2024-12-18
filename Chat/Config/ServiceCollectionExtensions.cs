using Chat.Domain;
using Chat.Domain.Entities;
using Chat.Domain.Interface;
using Chat.Infrastructure.Data;
using Chat.Infrastructure.Kafka;
using Chat.Infrastructure.Services;
using Chat.Infrastructure.SignalR;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Chat.Config
{
    public static class ServiceCollectionExtensions
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
        public static IServiceCollection AddKafka(this IServiceCollection services)
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
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string \"DefaultConnection\" not found.");
            
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            return services;
        }

        /// <summary>
        /// Регистрирует сервисы Hangfire.
        /// </summary>
        /// <param name="services">Коллекция служб.</param>
        /// <param name="configuration">Конфигурация приложения.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddHangfire(this IServiceCollection services, IConfiguration configuration)
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
        /// Регистрирует Swagger.
        /// <param name="services">Коллекция служб.</param>
        /// <returns>Коллекция служб.</returns>
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            return services.AddSwaggerGen();
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
