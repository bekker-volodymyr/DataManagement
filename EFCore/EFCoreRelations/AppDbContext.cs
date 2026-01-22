using EFCoreRelations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EFCoreRelations
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB; Initial Catalog=EFRelations;")
                .LogTo(Console.WriteLine, LogLevel.Information) // логування в консоль
                .EnableSensitiveDataLogging(); // показує реальні значення параметрів SQL запитів ('Іван', 25 тощо). 
                                               // У продакшн-режимі краще не вмикати, бо може злити чутливі дані в логи.
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Додаємо унікальний індекс
            modelBuilder.Entity<Student>().HasIndex(s => s.Email).IsUnique();

            modelBuilder.Entity<Group>().HasIndex(g => g.Name).IsUnique();

            modelBuilder.Entity<Teacher>().ToTable(t => t.HasCheckConstraint("CK_Teacher_Salary", "[Salary] > 0"));

            modelBuilder.Entity<Student>().De

            // Простіший але застарілий варіант
            //modelBuilder.Entity<Teacher>().HasCheckConstraint("CK_Teacher_Salary", "[Salary] > 0");
        }
    }
}
