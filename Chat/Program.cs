using Chat.Config;
using Chat.ConsoleWorker.Config;
using Chat.Infrastructure.Data;
using Chat.Infrastructure.SignalR.Hubs;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Chat
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddConfigs(builder.Configuration)
                    .AddKafka()
                    .AddDatabase(builder.Configuration)
                    .AddHangfire(builder.Configuration)
                    .AddMessageObservers()
                    .AddConsoleWorker()
                    .AddSwagger()
                    .AddDifferentServices();

            var app = builder.Build();

            var hangfireConnectionString = builder.Configuration.GetConnectionString("Hangfire")?? throw new InvalidOperationException("Connection string \"Hangfire\" not found.");
            using (var connect = new HangfireDbContext(hangfireConnectionString))
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
