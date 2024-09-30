using Chat.Data;
using Chat.Services;
using Chat.Services.Hubs;
using Chat.Services.Kafka;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;

namespace Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<KafkaConfig>(builder.Configuration.GetSection(KafkaConfig.Position));
            builder.Services.Configure<ChatConfig>(builder.Configuration.GetSection(ChatConfig.Position));
            builder.Services.AddSignalR();

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHangfire(h =>
            {
                h.UsePostgreSqlStorage(conf =>
                conf.UseNpgsqlConnection(builder.Configuration.GetConnectionString("Hangfire")));
            });
            builder.Services.AddHangfireServer(options =>
            {
                options.WorkerCount = 5;
                options.SchedulePollingInterval = TimeSpan.FromSeconds(1);
            });
            builder.Services.AddHangfireServer();

            builder.Services.AddSingleton<IProducer, Producer>();
            builder.Services.AddSingleton<SignalRMessageObserver>();

            builder.Services.AddSingleton<IMessageSubject, MessageSubject>(serviceProvider =>
            {
                var messageSubject = new MessageSubject();

                var signalRObserver = serviceProvider.GetRequiredService<SignalRMessageObserver>();

                messageSubject.Attach(signalRObserver);

                return messageSubject;
            });


            builder.Services.AddHostedService<ConsumerHostedService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add services to the container.
            builder.Services.AddRazorPages();

            using (var connect = new HangfireDbContext(builder.Configuration.GetConnectionString("Hangfire")))
            {
                connect.Database.EnsureCreated();
            }

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseHangfireDashboard("/Hang");
            app.MapHub<ChatHub>("/Chat");

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
