using EFCoreFluentApi.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace EFCoreFluentApi
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }

        private readonly string _connectionString = null!;

        public AppDbContext()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appconfig.json").Build();

            _connectionString = config.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .Property(s => s.Email)
                .HasColumnType("NVARCHAR")
                .HasMaxLength(50);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();
        }
    }
}
