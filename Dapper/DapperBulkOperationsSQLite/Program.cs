using Dapper;
using Z.Dapper.Plus;
using Microsoft.Data.Sqlite;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;

}

namespace DapperBulkOperationsSQLite
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize Dapper Plus
            DapperPlusManager.Entity<User>().Table("Users").Key(u => u.Id);

            // Example usage of bulk insert
            using (var connection = new SqliteConnection("Data Source=users.db"))
            {
                connection.Open();

                connection.Execute("CREATE TABLE IF NOT EXISTS Users (Id TEXT PRIMARY KEY, Name TEXT NOT NULL)");

                var entities = new List<User>
                {
                    new User { Name = "Alice" },
                    new User { Name = "Bob" }
                };

                connection.BulkInsert(entities);
            }
        }
    }
}