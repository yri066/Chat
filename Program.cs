using Chat.ConsoleWorker.Interface;
using Chat.ConsoleWorker.Workers;
using Chat.Data;
using Chat.Interface;
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

            builder.Services.AddConfigs(builder.Configuration)
                    .AddKafkaServices(builder.Configuration)
                    .AddDatabaseNpgsql(builder.Configuration)
                    .AddHangfireServices(builder.Configuration)
                    .AddMessageObservers()
                    .AddConsoleWorkers()
                    .AddSwagger()
                    .AddDifferentServices();

            var app = builder.Build();

            using (var connect = new HangfireDbContext(builder.Configuration.GetConnectionString("Hangfire")))
            {
                connect.Database.EnsureCreated();
            }

            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

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
            app.MapControllers();
            app.MapRazorPages();

            app.Run();
        }
    }
}
