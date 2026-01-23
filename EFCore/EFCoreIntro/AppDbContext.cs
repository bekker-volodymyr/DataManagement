using EFCoreIntro.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCoreIntro
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        private readonly string? _connectionString;

        public AppDbContext()
        {
            // читаємо конфіг
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // читаємо конфіг
            var config = new ConfigurationBuilder()
                .AddJsonFile("appconfig.json")
                .Build();

            optionsBuilder.UseSqlServer(_connectionString);
        }
    }
}
