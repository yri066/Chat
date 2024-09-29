using Chat.Services.Hubs;
using Chat.Services.Kafka;

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

            builder.Services.AddSingleton<IProducer, Producer>();
            builder.Services.AddHostedService<ConsumerHostedService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add services to the container.
            builder.Services.AddRazorPages();

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

            app.MapHub<ChatHub>("/Chat");

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
