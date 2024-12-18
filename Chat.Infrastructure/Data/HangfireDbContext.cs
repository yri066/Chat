using Microsoft.EntityFrameworkCore;

namespace Chat.Infrastructure.Data
{
    public class HangfireDbContext : DbContext
    {
        private string _connectionString;
        public HangfireDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
