using Dapper;
using Z.Dapper.Plus;
using MySqlConnector;
using DapperBulkOperations.Entities;

namespace DapperBulkOperations;

public class Program
{
    private readonly static string _masterConnectionString = "Server=localhost;User=appuser;Password=strongpassword;";
    private readonly static string _connectionString = "Server=localhost;Database=library;User=appuser;Password=strongpassword;";

    public static void Main(string[] args)
    {
        // реєструємо тип (обов’язково!)
        DapperPlusManager.Entity<Author>().Table("Authors").Key(a => a.Id);
        DapperPlusManager.Entity<Book>().Table("Books").Key(b => b.Id);

        // Налаштування Dapper Plus:
        DapperPlusManager.Entity<Visitor>()
            .Table("Visitors")
            .Key(v => v.Id);
        DapperPlusManager.Entity<Passport>()
            .Table("Passports")
            .Key(p => p.Id);

        InitMySql();
        // Підключення до створеної бази даних
        using var dbConn = new MySqlConnection(_connectionString);
        dbConn.Open();
        CreateDatabase(dbConn);

        var authors = new List<Author>
        {
            new Author { Name = "Леся Українка", DateOfBirth = new DateTime(1871, 2, 25) },
            new Author { Name = "Іван Франко", DateOfBirth = new DateTime(1856, 8, 27) },
        };

        BulkAuthorsInsert(dbConn, authors);

        var updatedAuthors = new List<Author>
        {
            new Author { Id = 1, Name = "Леся Косач", DateOfBirth = new DateTime(1871, 2, 25) },
            new Author { Id = 2, Name = "Іван Якович Франко", DateOfBirth = new DateTime(1856, 8, 27) },
        };

        BulkUpdateAuthors(dbConn, updatedAuthors);

        var deleteAuthors = new List<Author>
        {
            new Author { Id = 1 },
            new Author { Id = 3 }
        };

        dbConn.BulkDelete(deleteAuthors);

        var authorsToMerge = new List<Author>
        {
            new Author { Id = 1, Name = "Леся Українка (оновлена)", DateOfBirth = new DateTime(1871, 2, 25) },  // оновиться
            new Author { Name = "Василь Стефаник", DateOfBirth = new DateTime(1871, 5, 14) }  // вставиться (Id не задано)
        };

        dbConn.BulkMerge(authorsToMerge);

    }

    private static void BulkUpdateAuthors(MySqlConnection connection, List<Author> updatedAuthors)
    {
        connection.BulkUpdate(updatedAuthors);
    }

    private static void BulkAuthorsInsert(MySqlConnection connection, List<Author> authors)
    {
        // виконуємо bulk insert
        connection.BulkInsert(authors);
    }

    private static void InitMySql()
    {
        // Підключення до MySQL сервера
        using var conn = new MySqlConnection(_masterConnectionString);
        conn.Open();

        // Створення бази даних, якщо вона не існує
        conn.Execute("CREATE DATABASE IF NOT EXISTS library;");

        Console.WriteLine("Базу створено й підключено");

    }

    private static void CreateDatabase(MySqlConnection connection)
    {
        var createTableQuery = @"
        CREATE TABLE IF NOT EXISTS Authors (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            DateOfBirth DATE NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Books (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Title VARCHAR(255) NOT NULL,
            AuthorId INT NOT NULL,
            FOREIGN KEY (AuthorId) REFERENCES Authors(Id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS Visitors (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            Name VARCHAR(255) NOT NULL,
            PhoneNumber VARCHAR(50) NOT NULL,
            DateOfBirth DATE NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Passports (
            Id INT AUTO_INCREMENT PRIMARY KEY,
            PassportNumber VARCHAR(100) NOT NULL,
            VisitorId INT NOT NULL UNIQUE,
            FOREIGN KEY (VisitorId) REFERENCES Visitors(Id) ON DELETE CASCADE
        );

        CREATE TABLE IF NOT EXISTS BookLoans (
            BookId INT NOT NULL,
            VisitorId INT NOT NULL,
            LoanDate DATE NOT NULL,
            ReturnDate DATE,
            PRIMARY KEY (BookId, VisitorId, LoanDate),
            FOREIGN KEY (BookId) REFERENCES Books(Id) ON DELETE CASCADE,
            FOREIGN KEY (VisitorId) REFERENCES Visitors(Id) ON DELETE CASCADE
        );
        ";

        connection.Execute(createTableQuery);
    }
}