using Microsoft.EntityFrameworkCore;

namespace EFCoreWithSqlite
{
    public class AppDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=EfDemo.db");
        }
    }
}
